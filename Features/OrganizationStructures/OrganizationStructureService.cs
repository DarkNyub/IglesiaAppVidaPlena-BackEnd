namespace IglesiaBackend.Features.OrganizationStructures;

public class OrganizationStructureService
{
    private readonly OrganizationStructureRepository _repository;

    public OrganizationStructureService(OrganizationStructureRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<OrganizationStructureDto>> GetAllAsync()
    {
        var list = await _repository.GetAllAsync();
        return list.Select(OrganizationStructureMapper.ToDto).ToList();
    }

    public async Task<List<OrganizationStructureDto>> GetChildrenAsync(int parentId)
    {
        var list = await _repository.GetChildrenAsync(parentId);
        return list.Select(OrganizationStructureMapper.ToDto).ToList();
    }

    public async Task<OrganizationStructureDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : OrganizationStructureMapper.ToDto(entity);
    }

    public async Task<OrganizationStructureDto> CreateAsync(OrganizationStructureCreateUpdateDto dto)
    {
        // Validación básica: Si manda ParentId, asegurarse que no sea 0 o inválido lógico
        if (dto.ParentId.HasValue && dto.ParentId.Value <= 0) dto.ParentId = null;

        var entity = OrganizationStructureMapper.ToEntity(dto);
        await _repository.CreateAsync(entity);

        return OrganizationStructureMapper.ToDto(entity);
    }

    public async Task<bool> UpdateAsync(int id, OrganizationStructureCreateUpdateDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        // Validación de Integridad: Una estructura no puede ser padre de sí misma
        if (dto.ParentId == id)
        {
            throw new InvalidOperationException("Una estructura no puede ser su propio padre.");
        }

        OrganizationStructureMapper.UpdateEntity(entity, dto);
        await _repository.UpdateAsync(entity);

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        // 1. REGLA: No borrar si tiene Hijos (Sub-Estructuras) activos
        bool hasChildren = await _repository.HasActiveChildrenAsync(id);
        if (hasChildren)
            throw new InvalidOperationException("No se puede eliminar: Esta estructura contiene sub-divisiones activas. Debe reasignarlas o eliminarlas primero.");

        // 2. REGLA: No borrar si tiene Miembros activos asignados
        bool hasMembers = await _repository.HasActiveMembersAsync(id);
        if (hasMembers)
            throw new InvalidOperationException("No se puede eliminar: Existen miembros activos asignados a esta estructura. Debe transferirlos primero.");

        // Si pasa las validaciones -> Soft Delete
        await _repository.DeleteAsync(entity);
        return true;
    }

    // Y el Restore genérico que ya conoces
    public async Task<bool> RestoreAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        // Opcional: Podrías validar que el Padre de este nodo esté activo antes de revivirlo,
        // pero por ahora, revivirlo a secas está bien.
        entity.IsDeleted = false;
        await _repository.UpdateSimpleAsync(entity);
        return true;
    }

    public async Task<bool> CloneDeepAsync(int id, CloneOrganizationStructureDto cloneDto)
    {
        var originalRoot = await _repository.GetTreeByIdAsync(id);
        if (originalRoot == null) return false;

        // Método recursivo local para duplicar nodos
        async Task CloneNodeRecursive(OrganizationStructure originalNode, int? newParentId, string? overrideName = null, string? overrideDescription = null)
        {
            // Creamos la copia
            var newNode = new OrganizationStructure
            {
                Name = overrideName ?? originalNode.Name, // Usamos el nombre nuevo solo para la raíz
                Description = overrideDescription ?? originalNode.Description,
                OrganizationTypeId = originalNode.OrganizationTypeId,
                ParentId = newParentId,
                // Reiniciamos campos de auditoría automáticamente por EF Core
            };

            // Guardamos el nuevo nodo en la BD para que genere su nuevo ID (IDENTITY)
            await _repository.CreateAsync(newNode);

            // Llamada recursiva para los hijos
            foreach (var child in originalNode.Children)
            {
                // Para los hijos, mantenemos su nombre y descripción originales, 
                // pero los atamos al ID del nodo que acabamos de crear
                await CloneNodeRecursive(child, newNode.Id); 
            }
        }

        // Iniciamos la clonación desde la raíz, pasándole el nuevo nombre
        await CloneNodeRecursive(originalRoot, originalRoot.ParentId, cloneDto.NewName, cloneDto.NewDescription);

        return true;
    }
}