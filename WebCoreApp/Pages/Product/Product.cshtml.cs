using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.SqlClient;
using WebCoreApp.Pages.Model;

namespace WebCoreApp.Pages.Product
{
    public class ProductModel : PageModel
    {
        public List<ProductTbl> Product = new List<ProductTbl>();
        string path = @"Data Source=.\SQLEXPRESS;Initial Catalog=TEST;Integrated Security=True;";
        public void OnGet()
        {

            SqlConnection con = new SqlConnection(path);
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from Product", con);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                ProductTbl obj = new()
                {
                    Sno = dr.GetInt32(0),
                    ProductId = dr.GetString(1),
                    ProductName = dr.GetString(2),
                    Description = dr.GetString(3),
                    Price = dr.GetDouble(4)
                };
                Product.Add(obj);

            }

            con.Close();
        }
        public void OnPost()
        {
            string DMode = Request.Form["DeletemodeId"].ToString();
            string Mode = Request.Form["mode"].ToString();
            string prdid = Request.Form["IdmdlProductId"].ToString();
            string name = Request.Form["IdmdlProductName"].ToString();
            string descptn = Request.Form["IdmdlDescription"].ToString();
            double price = Convert.ToDouble(Request.Form["IdmdlPrice"]);

            bool modifiedstatus = false;
            string Strdata = ""; string PID = "";
            try
            {
                using (SqlConnection db = new SqlConnection(path))
                {
                    if (DMode != "")
                    {
                        db.Open();
                        SqlCommand cmd = new SqlCommand($"delete from Product where ProductId='{DMode.Split('~')[0]}'", db);
                        cmd.ExecuteNonQuery();
                        Strdata = "Product Deleted Successfully";
                    }
                    else
                    {
                        if (Mode == "Insert")
                        {
                            string objpid = GenerateProductId();
                            db.Open();
                            SqlCommand cmd = new SqlCommand($"select * from Product where ProductName='{name}'", db);
                            SqlDataReader dr = cmd.ExecuteReader();
                            if (dr.Read() == false)
                            {
                                dr.Close();
                                SqlCommand cmdupdate = new SqlCommand($"insert into Product (ProductId,ProductName,Description,Price) values ('{objpid}','{name}','{descptn}','{price}')", db);
                                cmdupdate.ExecuteNonQuery();
                                Strdata = "Product Added Successfully";
                            }
                            else
                            {
                                Strdata = "Product Already Exist";
                            }
                        }
                        else if (Mode == "Update")
                        {
                            db.Open();
                            SqlCommand cmd1 = new SqlCommand($"select top 1 * from Product where ProductId='{prdid}'", db);
                            SqlDataReader dr = cmd1.ExecuteReader();
                            while (dr.Read())
                            {
                                if (dr.GetString(2) != name)
                                {
                                    modifiedstatus = true;
                                }
                                else if (dr.GetString(3) != descptn)
                                {
                                    modifiedstatus = true;
                                }
                                else if (dr.GetDouble(4) != price)
                                {
                                    modifiedstatus = true;
                                }
                                else
                                {
                                    Strdata = "No changes found!";
                                }
                            }
                            dr.Close();

                            if (modifiedstatus == true)
                            {
                                SqlCommand cmdupdate = new SqlCommand($"update Product set ProductName='{name}',Description='{descptn}',Price='{price}' where ProductId='{prdid}'", db);
                                cmdupdate.ExecuteNonQuery();
                                Strdata = "Product Modified Successfully";
                            }
                        }
                    }
                }
                Response.Redirect("/Product/Product");
            }
            catch (Exception ex)
            {

            }
        }

        public string GenerateProductId()
        {
            string Series = ""; int val = 0;
            using (SqlConnection dbcontext = new SqlConnection(path))
            {
                dbcontext.Open();
                SqlCommand cmd = new SqlCommand("select ProductId from Product", dbcontext);
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                List<string> prdcnt = new List<string>();
                foreach (DataRow dr in dt.Rows)
                {
                    prdcnt.Add(dr[0].ToString().Replace("P", ""));
                }
                // prdcnt = dbcontext.Products.Select(t => t.ProductId.Replace("P", "")).ToList();
                if (prdcnt.Count > 0)
                {
                    val = Convert.ToInt32(prdcnt.AsEnumerable().OrderBy(t => Convert.ToInt32(t)).LastOrDefault());
                }
                Series = "P" + (val + 1).ToString().PadLeft(4, '0');
            }
            return Series;
        }
    }
}