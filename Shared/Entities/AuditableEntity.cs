namespace IglesiaBackend.Shared.Entities;

public abstract class AuditableEntity
{
    public DateTime CreateAt { get; set; }
    public int? IdUserCreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }
    public int? IdUserUpdateAt { get; set; }
    
    // --- NUEVOS CAMPOS PARA SOFT DELETE (En todas las tablas) ---
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public int? IdUserDeletedAt { get; set; }
}
