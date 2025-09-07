using AutoMapper;
using GEEKS.Dto;
using GEEKS.Models;

namespace GEEKS.Mappings
{
    /// <summary>
    /// Perfil de mapeo para productos
    /// </summary>
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            // Mapeo de Product a ProductResponseDTO
            CreateMap<Product, ProductResponseDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty));

            // Mapeo de Product a ProductListDTO
            CreateMap<Product, ProductListDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty));

            // Mapeo de CreateProductDTO a Product
            CreateMap<CreateProductDTO, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => "Active"))
                .ForMember(dest => dest.CreatedAtDateTime, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAtDateTime, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore());

            // Mapeo de UpdateProductDTO a Product (solo campos no nulos)
            CreateMap<UpdateProductDTO, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAtDateTime, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAtDateTime, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // Mapeo de Category a CategoryListDTO
            CreateMap<Category, CategoryListDTO>()
                .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products.Count(p => p.State == "Active")));

            // Mapeo de Category a CategoryDTO
            CreateMap<Category, CategoryDTO>();

            // Mapeo de CreateCategoryDTO a Category
            CreateMap<CreateCategoryDTO, Category>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => "Active"))
                .ForMember(dest => dest.CreatedAtDateTime, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAtDateTime, opt => opt.Ignore())
                .ForMember(dest => dest.Products, opt => opt.Ignore());

            // Mapeo de UpdateCategoryDTO a Category
            CreateMap<UpdateCategoryDTO, Category>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAtDateTime, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAtDateTime, opt => opt.Ignore())
                .ForMember(dest => dest.Products, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
