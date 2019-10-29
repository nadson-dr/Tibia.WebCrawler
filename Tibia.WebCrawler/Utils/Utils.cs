using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace Tibia.WebCrawler.Utils
{
    public class Utils
    {
        public static object preencherObjeto(Type objType, DataRow _dr)
        {
            var properties = objType.GetProperties();

            try
            {
                object x = Activator.CreateInstance(objType);

                foreach (PropertyInfo info in properties)
                {
                    if (!info.PropertyType.FullName.Contains("Collections"))//Não preencher as Listas
                    {
                        if (_dr.Table.Columns.Contains(info.Name))
                        {
                            var valor = _dr[info.Name]; //elemento.Elements().Where(c => c.Name.LocalName == info.Name).FirstOrDefault();

                            if (!string.IsNullOrEmpty(valor.ToString()))
                            {
                                Type u = Nullable.GetUnderlyingType(Type.GetType(info.PropertyType.FullName)); //Verifica o tipo padrão ignorando se o mesmo é nullable
                                if (u != null)
                                {
                                    info.SetValue(x, Convert.ChangeType(valor, u), null);
                                }
                                else
                                {
                                    info.SetValue(x, Convert.ChangeType(valor, Type.GetType(info.PropertyType.FullName)), null);
                                }

                            }
                        }

                    }
                }
                return x;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
