using SV21T1020206.BusinessLayers;
using SV21T1020206.DataLayers;
using SV21T1020206.DataLayers.SQLServer;
using SV21T1020206.DomainModels;

namespace SV21T1020206.BusinessLayers
{
    public static class ProductDataService
    {
        private static readonly IProductDAL productDB;

        static ProductDataService()
        {
            string connectionString = Configuration.ConnectionString;
            productDB = new ProductDAL(connectionString);
        }
        public static List<Product> ListProducts(string searchValue = "")
        {
            return productDB.List(searchValue: searchValue);
        }

        public static List<Product> ListProducts(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "", int categoryId = 0, int supplierId = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            rowCount = productDB.Count(searchValue, categoryId, supplierId, minPrice, maxPrice);
            return productDB.List(page, pageSize, searchValue, categoryId, supplierId, minPrice, maxPrice);
        }

        /// <summary>
        /// Lấy thông tin 1 mặt hàng
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static Product? GetProduct(int productID)
        {
            return productDB.Get(productID);
        }
        /// <summary>
        /// Thêm mới 1 mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddProduct(Product data)
        {
            return productDB.Add(data);
        }
        /// <summary>
        /// Cập nhật thông tin 1 mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateProduct(Product data)
        {
            return productDB.Update(data);
        }
        /// <summary>
        /// Xóa 1 mặt hàng (nếu không có dữ liệu liên quan)
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static bool DeleteProduct(int productID)
        {
            if (productDB.InUsed(productID)) return false;
            return productDB.Delete(productID);
        }
        /// <summary>
        /// Kiểm tra 1 mặt hàng có dữ liệu liên quan hay không
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static bool InUsedProduct(int productID)
        {
            return productDB.InUsed(productID);
        }

        /// <summary>
        /// Lấy danh sách ảnh của 1 mặt hàng
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static List<ProductPhoto> ListOfProductPhotos(int productID)
        {
            return productDB.ListPhotos(productID).ToList();
        }
        /// <summary>
        /// Lấy thông tin 1 ảnh của 1 mặt hàng
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static ProductPhoto? GetProductPhoto(long photoID)
        {
            return productDB.GetPhoto(photoID);
        }
        /// <summary>
        /// Thêm mới 1 ảnh cho 1 mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static long AddProductPhoto(ProductPhoto data)
        {
            return productDB.AddPhoto(data);
        }
        /// <summary>
        /// Cập nhật 1 ảnh cho 1 mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateProductPhoto(ProductPhoto data)
        {
            return productDB.UpdatePhoto(data);
        }
        /// <summary>
        /// Xóa 1 ảnh cho 1 mặt hàng
        /// </summary>
        /// <param name="photoID"></param>
        /// <returns></returns>
        public static bool DeleteProductPhoto(long photoID)
        {
            return productDB.DeletPhoto(photoID);
        }

        /// <summary>
        /// Lấy danh sách thuộc tính cho 1 mặt hàng
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static List<ProductAttribute> ListOfProductAttributes(int productID)
        {
            return productDB.ListAttributes(productID).ToList();
        }
        /// <summary>
        /// Lấy thông tin 1 thuộc tính cho 1 mặt hàng
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static ProductAttribute? GetProductAttribute(long attributeID)
        {
            return productDB.GetAttribute(attributeID);
        }
        /// <summary>
        /// Thêm mới 1 thuộc tính cho 1 mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static long AddProductAttribute(ProductAttribute data)
        {
            return productDB.AddAttribute(data);
        }
        /// <summary>
        /// Cập nhật thông tin 1 thuộc tính cho 1 mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateProductAttribute(ProductAttribute data)
        {
            return productDB.UpdateAttribute(data);
        }
        /// <summary>
        /// Xóa 1 thuộc tính cho 1 mặt hàng
        /// </summary>
        /// <param name="attributeID"></param>
        /// <returns></returns>
        public static bool DeleteProductAttribute(long attributeID)
        {
            return productDB.DeleteAttribute(attributeID);
        }
    }
}
