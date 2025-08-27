namespace GEEKS.Dto
{
    // DTO para crear un producto
    public class CreateProductDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int Stock { get; set; }
        public int MinStock { get; set; } = 5;
        public string SKU { get; set; } = string.Empty;
        public string? MainImage { get; set; }
        public List<string> Images { get; set; } = new List<string>();
        public int CategoryId { get; set; }
        public string? Brand { get; set; }
        public bool IsFeatured { get; set; } = false;
        public decimal Weight { get; set; } = 0;
        public decimal Length { get; set; } = 0;
        public decimal Width { get; set; } = 0;
        public decimal Height { get; set; } = 0;
    }

    // DTO para actualizar un producto
    public class UpdateProductDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ShortDescription { get; set; }
        public decimal? Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int? Stock { get; set; }
        public int? MinStock { get; set; }
        public string? SKU { get; set; }
        public string? MainImage { get; set; }
        public List<string>? Images { get; set; }
        public int? CategoryId { get; set; }
        public string? Brand { get; set; }
        public bool? IsFeatured { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public string? State { get; set; }
    }

    // DTO para respuesta de producto individual
    public class ProductResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int Stock { get; set; }
        public int MinStock { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string? MainImage { get; set; }
        public List<string> Images { get; set; } = new List<string>();
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? Brand { get; set; }
        public string State { get; set; } = string.Empty;
        public bool IsFeatured { get; set; }
        public decimal Weight { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public DateTime CreatedAtDateTime { get; set; }
        public DateTime? UpdatedAtDateTime { get; set; }
    }

    // DTO para listado de productos con información básica
    public class ProductListDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int Stock { get; set; }
        public string? MainImage { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? Brand { get; set; }
        public bool IsFeatured { get; set; }
        public string State { get; set; } = string.Empty;
    }

    // DTO para filtros de búsqueda
    public class ProductFilterDTO
    {
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public string? Brand { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? InStockOnly { get; set; }
        public bool? FeaturedOnly { get; set; }
        public string? SortBy { get; set; } = "Name"; // Name, Price, Stock, CreatedAt
        public string? SortOrder { get; set; } = "asc"; // asc, desc
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    // DTO para respuesta paginada
    public class PaginatedResponseDTO<T>
    {
        public List<T> Data { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }
}

