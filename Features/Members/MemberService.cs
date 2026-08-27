using System.Text.Json;
using System.Text.Json.Nodes; // <--- NECESARIO

namespace IglesiaBackend.Features.Members;

public class MemberService
{
    private readonly MemberRepository _repository;

    public MemberService(MemberRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<MemberDto>> GetAllAsync()
    {
        var members = await _repository.GetAllAsync();
        return members.Select(MemberMapper.ToDto).ToList();
    }

    public async Task<MemberDto?> GetByIdAsync(int id)
    {
        var member = await _repository.GetByIdAsync(id);
        return member == null ? null : MemberMapper.ToDto(member);
    }

    public async Task<MemberDto> CreateAsync(MemberCreateUpdateDto dto)
    {
        // 1. Convertimos el DTO a Entidad (Datos básicos)
        var entity = MemberMapper.ToEntity(dto);

        // 2. CORRECCIÓN: Pasamos la entidad Y la lista de roles al repositorio
        // El repositorio se encargará de guardar el miembro y luego sus relaciones
        await _repository.CreateAsync(entity, dto.Roles);

        return MemberMapper.ToDto(entity);
    }

    public async Task<bool> UpdateAsync(int id, MemberCreateUpdateDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        // 1. Actualizamos los datos básicos en la entidad
        MemberMapper.UpdateEntity(entity, dto);

        // 2. CORRECCIÓN: Pasamos la entidad Y la nueva lista de roles
        // El repositorio borrará los roles viejos y pondrá los nuevos
        await _repository.UpdateAsync(entity, dto.Roles);

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        await _repository.DeleteAsync(entity);
        return true;
    }

    public async Task<bool> ToggleStatusAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        // 1. Determinar textos
        bool isReactivating = entity.IsDeleted;
        string actionText = isReactivating
            ? $"Accion: Dado de alta por admin"
            : $"Accion: Dado de baja por admin";
        string dateText = $"Fecha: {DateTime.Now:yyyy-MM-dd HH:mm}";

        // 2. MANIPULACIÓN DEL JSON CON JsonNode (Mucho más fácil)
        JsonNode rootNode;

        // Si es nulo o vacío, creamos un objeto nuevo vacio
        if (entity.ExtraData == null)
        {
            rootNode = new JsonObject();
        }
        else
        {
            try
            {
                rootNode = JsonNode.Parse(entity.ExtraData.RootElement.GetRawText());
            }
            catch
            {
                // Si el JSON está corrupto, iniciamos uno nuevo
                rootNode = new JsonObject();
            }
        }

        // 🔥 CORRECCIÓN AQUÍ 🔥
        // Intentamos convertirlo a Objeto. Si rootNode es un Array (como "[null]"), esto devolverá null.
        var jsonObject = rootNode as JsonObject;

        // Si no era un objeto (era null o era un Array basura), creamos uno limpio.
        if (jsonObject == null)
        {
            jsonObject = new JsonObject();
        }

        // 3. Obtener o Crear el Array "items"
        if (!jsonObject.ContainsKey("items") || jsonObject["items"] == null)
        {
            jsonObject["items"] = new JsonArray();
        }

        var itemsArray = jsonObject["items"]!.AsArray();

        // 4. Agregar el nuevo log
        // JsonNode permite agregar objetos anónimos directamente
        itemsArray.Add(new
        {
            category = "System Log",
            key = "Status Change",
            value = $"{actionText} | {dateText}"
        });

        // 5. Guardar de vuelta en la entidad (Convertir Node -> Document)
        entity.ExtraData = JsonDocument.Parse(jsonObject.ToJsonString());

        if (isReactivating)
        {
            // SI VAMOS A REACTIVAR:
            entity.IsDeleted = false;
            if (entity.User != null) entity.User.IsDeleted = false;
            await _repository.UpdateSimpleAsync(entity); // Guardado normal
        }
        else
        {
            // 🔥 SI VAMOS A DAR DE BAJA:
            // Usamos DeleteAsync para que el DbContext dispare la auditoría (DeletedAt, IdUserDeletedAt)
            // Primero, actualizamos el ExtraData (el log) usando Update Simple
            await _repository.UpdateSimpleAsync(entity);

            // Y luego lo mandamos a "borrar"
            await _repository.DeleteAsync(entity);

            // Nota: Para el 'entity.User', lo ideal sería que también lo pases 
            // por un _userRepository.DeleteAsync(entity.User) si quisieras auditarlo,
            // pero si usas el Update manual como lo tenías, al menos funcionará.
            if (entity.User != null)
            {
                entity.User.IsDeleted = true;
                // Asumiendo que DbContext guardará esto como un Update para el usuario
            }
        }

        return true;
    }
    public async Task<int> BulkCreateAsync(List<MemberCreateUpdateDto> dtos)
    {
        var entities = new List<Member>();

        foreach (var dto in dtos)
        {
            var entity = MemberMapper.ToEntity(dto);

            // Si en el Excel venían roles, los amarramos de una vez
            if (dto.Roles != null && dto.Roles.Any())
            {
                foreach (var role in dto.Roles)
                {
                    entity.OrganizationMemberships.Add(new IglesiaBackend.Features.OrganizationMembers.OrganizationMember
                    {
                        OrganizationStructureId = role.StructureId,
                        ChurchFunctionRoleId = role.RoleId,
                        IsActive = true,
                        IsDeleted = false
                    });
                }
            }
            entities.Add(entity);
        }

        return await _repository.BulkCreateAsync(entities);
    }
}