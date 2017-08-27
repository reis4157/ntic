using Autofac;
using HepsiBenimMi.Utility;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Services.Media;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Nop.Data.Generate
{
    class Program
    {
        static void Main(string[] args)
        {
            //ProductUpdate();
            //ProductUpdateCategory();
            //InsertCategory();
            //InsertPicture();
            //UpdatePicSeao();
        }

        private static void UpdatePicSeao()
        {
            String query = @"select  p.Id, u.Slug, p.SeoFilename
                          FROM UrlRecord u, Picture p , Product_Picture_Mapping m 
                        where  p.Id = m.PictureId
                        and m.ProductId = u.EntityId 
                        and u.EntityName = 'Product'";
            foreach (DataRow item in dbIslemler.GetDataTable(query).Rows)
            {
                String upd = String.Format("update Picture set SeoFilename = '{0}' where Id = {1}", item["Slug"].ToString().Replace("'", "''"), item["Id"]);
                dbIslemler.GetExecuteScalar(upd);
            }
        }

        private static void InsertPicture()
        {
            String query = @"select Href, p.Id, f.Category,f.PicHref, f.Name from ProductFarmasi f,  Product p where f.ProductId = p.Sku";
            foreach (DataRow item in dbIslemler.GetDataTable(query).Rows)
            {
                String name = item["Name"].ToString();

                Byte[] image = WebOperations.GetImage(item["PicHref"].ToString());
                String imageBinary = ByteArrayToString(image);

                String seoName = FireSql(image, name);
                Object sonuc = dbIslemler.GetExecuteScalar(String.Format("select Max(Id) FROM [DOGALCOSMO].[dbo].[Picture] where SeoFilename = '{0}'", seoName.Replace("'", "''")));


                String imageMap = @"INSERT INTO [DOGALCOSMO].[dbo].[Product_Picture_Mapping]
                               ([ProductId]
                               ,[PictureId]
                               ,[DisplayOrder])
                         VALUES
                               ({0}
                               ,{1}
                               ,0)";
                String insertImageMap = String.Format(imageMap, item["Id"].ToString(), sonuc.ToInt());
                dbIslemler.GetExecuteScalar(insertImageMap);
            }
        }

        private static String FireSql(Byte[] imageBinary, String name)
        {
            String seoName = GetSeo(name);
            String insert = @"  INSERT INTO [DOGALCOSMO].[dbo].[Picture]
                                   ([PictureBinary]
                                   ,[MimeType]
                                   ,[SeoFilename]
                                   ,[AltAttribute]
                                   ,[TitleAttribute]
                                   ,[IsNew])
                             VALUES
                                   (@PictureBinary
                                   ,@MimeType
                                   ,@SeoFilename
                                   ,@AltAttribute
                                   ,@TitleAttribute
                                   ,@IsNew) ";
            SqlCommand SQLQuery = new SqlCommand(insert);
            SQLQuery.Parameters.Add("@PictureBinary", SqlDbType.VarBinary).Value = imageBinary;
            SQLQuery.Parameters.Add("@MimeType", SqlDbType.NVarChar).Value = "image/jpeg";
            SQLQuery.Parameters.Add("@SeoFilename", SqlDbType.NVarChar).Value = seoName;
            SQLQuery.Parameters.Add("@TitleAttribute", SqlDbType.NVarChar).Value = name;
            SQLQuery.Parameters.Add("@AltAttribute", SqlDbType.NVarChar).Value = name;
            SQLQuery.Parameters.Add("@IsNew", SqlDbType.Bit).Value = 0;
            dbIslemler.GetExecuteScalar(SQLQuery);
            return seoName;
        }

        private static String GetSeo(string name)
        {
            name = name.ToLower(CultureInfo.GetCultureInfo("tr-TR"));
            name = name.Replace(" ", "-");
            name = name.Replace("ğ", "g");
            name = name.Replace("ı", "i");
            name = name.Replace("ç", "c");
            name = name.Replace("ç", "c");
            name = name.Replace("---", "-");
            name = name.Replace("--", "-");
            name = name.Replace("--", "-");
            return name;
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:X2}", b);
            return hex.ToString();
        }

        private static void InsertCategory()
        {
            String productCategory = @"INSERT INTO [DOGALCOSMO].[dbo].[Product_Category_Mapping]
                                           ([ProductId]
                                           ,[CategoryId]
                                           ,[IsFeaturedProduct]
                                           ,[DisplayOrder])
                                     VALUES
                                           (@ProductId
                                           ,@CategoryId
                                           ,0
                                           ,0)";
            String query = @"select Href, p.Id, f.Category,f.PicHref from ProductFarmasi f,  Product p where f.ProductId = p.Sku";
            foreach (DataRow item in dbIslemler.GetDataTable(query).Rows)
            {
                String[] category = item["Category"].ToString().Split('/');
                foreach (var cat in category)
                {
                    String categoryUpdate = productCategory;
                    foreach (DataRow catId in dbIslemler.GetDataTable(String.Format("SELECT Id from [Category] where Name = '{0}'", cat.Replace("'", "''"))).Rows)
                    {
                        categoryUpdate = categoryUpdate.Replace("@CategoryId", catId["Id"].ToString());
                        categoryUpdate = categoryUpdate.Replace("@ProductId", item["Id"].ToString());
                        dbIslemler.GetExecuteNonQuery(categoryUpdate);
                    }
                }
            }
        }

        private static void ProductUpdateCategory()
        {
            foreach (DataRow item in dbIslemler.GetDataTable("select * from ProductFarmasi").Rows)
            {

                String categorySource = item["DescShort"].ToString();
                categorySource = categorySource.Substring(categorySource.IndexOf(">") + 1);
                if (!String.IsNullOrWhiteSpace(categorySource))
                {
                    String[] category = categorySource.Split('>');
                    String catLast = "";
                    foreach (var str in category)
                    {
                        catLast += "/" + str.Substring(0, str.IndexOf("\n")).Trim();
                    }
                    catLast = catLast.Substring(1);
                    String update = "update ProductFarmasi set category = @category@  where Id = @Id@ ";
                    update = update.Replace("@category@", "'" + catLast.Replace("'", "''") + "'");
                    update = update.Replace("@Id@", item["Id"].ToString());
                    dbIslemler.GetExecuteNonQuery(update);
                }
            }
        }

        private static void ProductUpdate()
        {
            foreach (DataRow item in dbIslemler.GetDataTable("select * from ProductFarmasi").Rows)
            {
                String href = item["Href"].ToString();
                string htmlSource = WebOperations.GetSourceCode(href);
                String categorySource = GetClassString(htmlSource, "breadcrumb", "<ul", "</ul >");
                String update = "update ProductFarmasi set category = @category@ , PicHref = @PicHref@, DescShort = @DescShort@ where Id = @Id@ ";
                update = update.Replace("@category@", "'" + GetCategory(categorySource).Replace("'", "''") + "'");
                update = update.Replace("@PicHref@", "'" + GetPicHref(htmlSource) + "'");
                update = update.Replace("@DescShort@", "'" + categorySource.Replace("'", "''").Trim() + "'");
                update = update.Replace("@Id@", item["Id"].ToString());
                dbIslemler.GetExecuteNonQuery(update);
            }
        }

        private static string GetPicHref(string htmlSource)
        {
            String image = GetClassString(htmlSource, "twitter:image", "content=\"", "\"");

            return image;
        }

        private static string GetCategory(string categorySource)
        {

            String category = "";
            while (categorySource.IndexOf("<a href") > -1)
            {
                categorySource = categorySource.Substring(categorySource.IndexOf("<a href") + 20);
                categorySource = categorySource.Substring(categorySource.IndexOf(">") + 1);
                if (categorySource.Length > 7 && categorySource.Substring(0, 7) == "<i clas")
                    continue;
                category += categorySource.Substring(0, categorySource.IndexOf("<")) + "/";
            }
            return category;
        }

        public class Product
        {
            public String Discount { get; set; }
            public String QuickHref { get; set; }
            public String Href { get; set; }
            public String PicHref { get; set; }
            public String Name { get; set; }
            public String DescLong { get; set; }
            public String DescShort { get; set; }
            public String PriceOld { get; set; }
            public String PriceNew { get; set; }
            public String ProductId { get; set; }
            public String Brand { get; set; }
            public String Gram { get; set; }
            public String Filename { get; set; }
        }

        private void updateProductFromFile_Click(object sender, EventArgs e)
        {
            List<String> fileNames = new List<string>();
            fileNames.Add("aktifyasam.txt");
            fileNames.Add("ciltbakimi.txt");
            fileNames.Add("drtuna.txt");
            fileNames.Add("kisiselbakim.txt");
            fileNames.Add("kokular.txt");
            fileNames.Add("makyaj.txt");
            fileNames.Add("sacbakim.txt");
            fileNames.Add("wipes.txt");

            List<Product> productList = new List<Product>();
            foreach (var fileName in fileNames)
            {
                String htmlCode = File.ReadAllText(fileName);
                String pattern = "product-layout";
                List<String> products = new List<string>();
                while (htmlCode.Contains(pattern))
                {
                    htmlCode = htmlCode.Substring(htmlCode.IndexOf(pattern) + pattern.Length);
                    products.Add(htmlCode.Substring(0, htmlCode.IndexOf("text/javascript")));
                }
                foreach (var item in products)
                {
                    Product prd = GetProduct(item);
                    prd.Filename = fileName;
                    productList.Add(prd);
                }
            }


            foreach (var item in productList)
            {
                dbIslemler.GetExecuteNonQuery(GetProductScript(item));
            }
        }

        private String GetProductScript(Product product)
        {
            String script = @"insert into ProductFarmasi (
                                Discount , QuickHref,
                                Href     , PicHref  ,
                                Name     , DescLong ,
                                PriceOld , PriceNew ,
                                ProductId, Brand    ,
                                Gram, Filename,
                                DescShort) values (
                                @Discount@ , @QuickHref@,
                                @Href@     , @PicHref@  ,
                                @Name@     , @DescLong@ ,
                                @PriceOld@ , @PriceNew@ ,
                                @ProductId@, @Brand@    ,
                                @Gram@, @Filename@,
                                @DescShort@)";

            script = script.Replace("@Discount@", "'" + product.Discount.Replace("'", "''") + "'");
            script = script.Replace("@QuickHref@", "'" + product.QuickHref.Replace("'", "''") + "'");
            script = script.Replace("@Href@", "'" + product.Href.Replace("'", "''") + "'");
            script = script.Replace("@PicHref@", "'" + product.PicHref.Replace("'", "''") + "'");
            script = script.Replace("@Name@", "'" + product.Name.Replace("'", "''") + "'");
            script = script.Replace("@DescLong@", "'" + product.DescLong.Replace("'", "''") + "'");
            script = script.Replace("@PriceOld@", "'" + product.PriceOld.Replace("'", "''") + "'");
            script = script.Replace("@PriceNew@", "'" + product.PriceNew.Replace("'", "''") + "'");
            script = script.Replace("@ProductId@", "'" + product.ProductId.Replace("'", "''") + "'");
            script = script.Replace("@Brand@", "'" + product.Brand.Replace("'", "''") + "'");
            script = script.Replace("@Gram@", "'" + product.Gram.Replace("'", "''") + "'");
            script = script.Replace("@Filename@", "'" + product.Filename.Replace("'", "''") + "'");
            script = script.Replace("@DescShort@", "'" + product.DescShort.Replace("'", "''") + "'");
            return script;
        }

        private Product GetProduct(string item)
        {
            Product product = new Product();
            if (item.Contains("sale_badge"))
            {
                String strDisc = item.Substring(item.IndexOf("sale_badge") + 12);
                strDisc = strDisc.Substring(0, strDisc.IndexOf("</div>"));
                strDisc = strDisc.Replace(" ", "")
                                 .Replace("\r", "")
                                 .Replace("\n", "")
                                 .Replace("%", "");
                product.Discount = strDisc;
            }

            String regexStr = @"<a[\s]+([^>]+)>((?:.(?!\<\/a\>))*.)<\/a>";
            MatchCollection matches = Regex.Matches(item, regexStr, RegexOptions.IgnorePatternWhitespace);
            for (int i = 0; i < matches.Count; i++)
            {
                String str = matches[i].ToString();
                if (str.Contains("<img"))
                {
                    str = str.Substring(str.IndexOf("href") + 6);
                    product.Href = str.Substring(0, str.IndexOf("\""));
                    str = str.Substring(str.IndexOf("src") + 5);
                    product.PicHref = str.Substring(0, str.IndexOf("\""));

                }
                if (i == 2)
                {
                    str = str.Substring(str.IndexOf(">") + 1);
                    product.Name = str.Substring(0, str.IndexOf("<")).Trim();
                }
                if (str.Contains("quickview"))
                {
                    str = str.Substring(str.IndexOf("href") + 6);
                    product.QuickHref = str.Substring(0, str.IndexOf("\""));
                    product.QuickHref = product.QuickHref.Replace("&amp;", "&");
                }
            }
            product.Brand = GetClassString(item, "brand main_font");
            product.PriceOld = GetClassString(item, "price-old");
            product.PriceNew = GetClassString(item, "price-new");
            product.DescShort = GetClassString(item, "description main_font");

            string htmlCode = WebOperations.GetSourceCode(product.QuickHref);
            String quickStr = htmlCode.Substring(htmlCode.IndexOf("description"));
            //quickStr = quickStr.Substring(10, quickStr.IndexOf("</div")); 
            product.DescLong = quickStr.Substring(quickStr.IndexOf("<p>") + 3);
            product.DescLong = product.DescLong.Substring(0, product.DescLong.IndexOf("</p>"));

            product.ProductId = GetClassString(quickStr, "Ürün Kodu:");
            product.Gram = GetClassString(quickStr, "Gramaj");
            return product;
        }

        public static String GetClassString(String item, String classStr, String start = ">", String end = "<")
        {
            String str = item;
            if (item.Contains(classStr))
            {
                str = str.Substring(str.IndexOf(classStr) + classStr.Length);
                str = str.Substring(str.IndexOf(start) + start.Length);
                str = str.Substring(0, str.IndexOf(end)).Trim();
                return str;
            }
            else
                return "";
        }
    }
}
