using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web; 
using System.Text;
using System.Collections;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Globalization;
using System.IO;



/// <summary>
/// DataBase ile ilgili yapýlan iþlemler bu class tan yapýlmakdýr : dbIslemler
/// </summary>
/// 
public class dbIslemler
{

  //  public SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString);
    public SqlConnection Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString);
    public SqlDataAdapter adp = new SqlDataAdapter();
    public DataSet ds;
    public SqlCommand cmd = new SqlCommand();
    public SqlDataReader dr;
    SqlTransaction tran;
    public static dbIslemler db = new dbIslemler();
    public static void conn_ac()
    {
        if (db.Connection.State == ConnectionState.Closed)
        {
           db.Connection.Open();
        }
    }
    public static void conn_kapat()
    {
        if (db.Connection.State == ConnectionState.Open)
        {
            db.Connection.Close();
        }
    }
    public static DataTable GetDataTable(string SQLQuery)
    {
        try
        {
            DataTable dt=new DataTable();
            SqlCommand myCmd = new SqlCommand(SQLQuery,db.Connection );
            SqlDataAdapter myDa = new SqlDataAdapter(myCmd);
            myDa.Fill(dt);
            return dt;
        }
        catch (Exception e)
        {
            throw e;
        }
        finally
        {
            conn_kapat();
        }
    }
    public static DataTable GetDataTable(SqlCommand SQLQuery)
    {
        try
        {
            DataTable dt = new DataTable();
            SQLQuery.Connection = db.Connection;
            SqlDataAdapter myDa = new SqlDataAdapter(SQLQuery);
            myDa.Fill(dt);
            return dt;
        }
        catch (Exception e)
        {
            throw e;
        }
        finally
        {
            conn_kapat();
        }
    }
    public static void GetExecuteNonQuery(string SQLQuery)
    {
        try
        {
            conn_ac();
            SqlCommand myCmd = new SqlCommand(SQLQuery, db.Connection);
            myCmd.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            throw e;
        }
        finally
        {
            conn_kapat();
        }
    }
    public static void GetExecuteNonQuery(SqlCommand SQLQuery)
    {
        try
        {

            conn_ac();
            db.tran = dbIslemler.db.Connection.BeginTransaction();
           // SQLQuery = dbIslemler.db.Connection.CreateCommand();
            SQLQuery.Transaction = db.tran;
            SQLQuery.Connection = db.Connection;
            SQLQuery.ExecuteNonQuery();
            db.tran.Commit();
            db.cmd.Parameters.Clear();
            
        }
        catch (Exception e)
        {
            throw e; 
        }
        finally
        {
            conn_kapat();
        }
    }
    public static object GetExecuteScalar(string SQLQuery)
    {
        object deger;
        try
        {
            conn_ac();
            SqlCommand myCmd = new SqlCommand(SQLQuery, db.Connection);
            deger = myCmd.ExecuteScalar();
            return deger;
        }
        catch (Exception e)
        {
            throw e;
        }
        finally
        {
            conn_kapat();
        }
    }
    public static object GetExecuteScalarNoResponse(string SQLQuery)
    {
        object deger;
        try
        {
            conn_ac();
            SqlCommand myCmd = new SqlCommand(SQLQuery, db.Connection);
            deger = myCmd.ExecuteScalar();
            return deger;
        }
        catch 
        {
            return null;
        }
        finally
        {
            conn_kapat();
        }
    }
    
    public static Boolean IsUsernameExists(String username)
    {
        SqlCommand cmd = new SqlCommand("select EMAIL from FIRMALAR where EMAIL='" + username.ToString()+ "'" ,db.Connection);
        try
        {
            conn_ac();
            Object result = cmd.ExecuteScalar();
            return result != null;
        }
        finally
        {
            conn_kapat();
        }
       
    }
    public static Boolean DoAdminLogin(String username,String password)
    {
        SqlCommand cmd = new SqlCommand("select PASSWORD from TBL_ADMIN where NAME='" + Islemler.ClearCharToDB(username) + "'", db.Connection);
        try
        {
            conn_ac();
            String sPassword = cmd.ExecuteScalar().ToString();
            return sPassword == password;
        }
        catch
        {
            return false;
        }
        finally
        {
            conn_kapat();
        }
    
    }
    public static Boolean DoLogin(String username, String password)
    {
        SqlCommand cmd = new SqlCommand("select SIFRE from FIRMALAR where EMAIL='" + Islemler.ClearCharToDB(username) + "'", db.Connection);
        try
        {
            conn_ac();
            String sPassword = cmd.ExecuteScalar().ToString();
            return sPassword == password;
        }
        catch
        {
            return false;
        }
        finally
        {
            conn_kapat();
        }
    }
    public static object GetExecuteScalar(SqlCommand SQLQuery)
    {
        object deger;
        try
        {
            SQLQuery.Connection = db.Connection;
            conn_ac();
            SqlCommand myCmd = SQLQuery;
            deger = myCmd.ExecuteScalar();
            return deger;
        }
        catch (Exception e)
        {
            throw e;
        }
        finally
        {
            conn_kapat();
        }
    }
    public static SqlDataReader GetExecuteReader(string SQLQuery)
    {
        SqlDataReader deger;

        try
        {
            conn_ac();
            SqlCommand myCmd = new SqlCommand(SQLQuery, db.Connection);
            deger = myCmd.ExecuteReader(CommandBehavior.CloseConnection);
            return deger;
        }
        catch (Exception e)
        {
            throw e;
        }
     
    }
    public static SqlDataReader GetExecuteReader(SqlCommand SQLQuery)
    {
        SqlDataReader deger;

        try
        {
            SQLQuery.Connection = db.Connection;
            conn_ac();
            SqlCommand myCmd = SQLQuery;
            deger = myCmd.ExecuteReader(CommandBehavior.CloseConnection);
            return deger;
        }
        catch (Exception e)
        {
            throw e;
        }
        
    }

}

public class Islemler
{
    public static string URLRewriteReplace(String str)
    {
        str = str.Replace("Ü", "U");
        str = str.Replace("Ç", "C");
        str = str.Replace("Ö", "O");
        str = str.Replace("Þ", "S");
        str = str.Replace("Ð", "G");
        str = str.Replace("Ý", "I");
        str = str.Replace("ü", "u");
        str = str.Replace("ö", "o");
        str = str.Replace("ç", "c");
        str = str.Replace("þ", "s");
        str = str.Replace("ý", "i");
        str = str.Replace("ð", "g");
        str = str.Replace(" ", "_");



        string newChars;
        string[] kotuchars = { "'", "-", "&", "@", "$", "£", ".", ",", "+", "/", "\\", "?", "*", ":", "&nbsp;", "(", ")", "=", "~", "<", ">", "|" };
        newChars = str;
        for (int i = 0; i <= kotuchars.Length - 1; i++)
        {
            newChars = newChars.Replace(kotuchars[i], "");
        }
        str = newChars;
        return str;
    }
    public static string KarakterTemizligi(string str, string Dil)
    {
        string newChars;
        string[] kotuchars = { ";", "--", "'", "select", "drop", "insert", "delete", "xp_", "sp_", "sp" };
        newChars = str.Trim();
        //TextInfo myTI = new CultureInfo(Dil, false).TextInfo;
        //newChars=myTI.ToLower(newChars);
        for (int i = 0; i <= kotuchars.Length - 1; i++)
        {
            newChars = newChars.Replace(kotuchars[i], "");
        }
        str = newChars.Trim();

        //str = str.Replace("ü", "u");
        //str = str.Replace("ö", "o");
        //str = str.Replace("ç", "c");
        //str = str.Replace("þ", "s");
        //str = str.Replace("ý", "i");
        //str = str.Replace("ð", "g");
        return str;
    }
    public static string ClearCharToDB(string str)
    {
        string newChars;
        string[] kotuchars = { ";", "--", "'" };
        newChars = str.Trim();
        for (int i = 0; i <= kotuchars.Length - 1; i++)
        {
            newChars = newChars.Replace(kotuchars[i], "");
        }
        str = newChars.Trim();
        return str;
    }
    public static string TextLowerAndFirstUpper(string str, int j)
    {
        str = str.ToLower();
        char[] stra = str.ToCharArray();
        for (int i = 0; i < stra.Length; i++)
        {
            if (i == 0)
            {
                str = string.Empty;
                str += stra[i].ToString().ToUpper();
            }
            else
            {
                str += stra[i].ToString();
            }
        }
        return str;
    }
    public static string Left(string param, int length)
    {
        //we start at 0 since we want to get the characters starting from the
        //left and with the specified lenght and assign it to a variable
        string result = param.Substring(0, length);
        //return the result of the operation
        return result;
    }
    public static string Right(string param, int length)
    {
        //start at the index based on the lenght of the sting minus
        //the specified lenght and assign it a variable
        string result = param.Substring(param.Length - length, length);
        //return the result of the operation
        return result;
    }

    public static string Mid(string param, int startIndex, int length)
    {
        //start at the specified index in the string ang get N number of
        //characters depending on the lenght and assign it to a variable
        string result = param.Substring(startIndex, length);
        //return the result of the operation
        return result;
    }

    public static string Mid(string param, int startIndex)
    {
        //start at the specified index and return all characters after it
        //and assign it to a variable
        string result = param.Substring(startIndex);
        //return the result of the operation
        return result;
    }

    private static string parametre;
    public static string QSKontrol
    {
        get
        {
            return parametre;
        }
        set
        {
            if (parametre != null)
            {
                if (parametre.Length > 0 & parametre.Length < 7 & Islemler.IsNumeric(parametre) & !(parametre == "0"))
                {
                    parametre = Convert.ToString(Math.Abs(Convert.ToDouble(parametre)));
                }
                else
                {
                    parametre = "-1";
                }
            }
        }

    }
    public static bool IsNumeric(string anyString)
    {
        if (anyString == null)
        {
            anyString = "";
        }
        if (anyString.Length > 0)
        {
            double dummyOut = new double();
            System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("tr-TR", true);

            return Double.TryParse(anyString, System.Globalization.NumberStyles.Any,
               cultureInfo.NumberFormat, out dummyOut);
        }
        else
        {
            return false;
        }
    }

}