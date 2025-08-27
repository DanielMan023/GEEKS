namespace GEEKS.Dto
{
    // DTO para crear una categoría
    public class CreateCategoryDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Image { get; set; }
    }

    // DTO para actualizar una categoría
    public class UpdateCategoryDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string? State { get; set; }
    }

    // DTO para respuesta de categoría
    public class CategoryResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string State { get; set; } = string.Empty;
        public DateTime CreatedAtDateTime { get; set; }
        public DateTime? UpdatedAtDateTime { get; set; }
        public int ProductCount { get; set; }
    }

    // DTO para listado de categorías
    public class CategoryListDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string State { get; set; } = string.Empty;
        public int ProductCount { get; set; }
    }
}

