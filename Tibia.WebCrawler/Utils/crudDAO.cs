using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace Tibia.WebCrawler.Utils
{
    public class crudDAO : IDisposable
    {
        string conexao = "";

        #region " Listar "

        public DataSet listarQuery(string query, Dictionary<string, object> dicParametros = null)
        {
            DataSet dsResultado = new DataSet();

            try
            {
                using (SqlConnection connection = new SqlConnection(Connection.bdConnection))
                {
                    SqlCommand command = new SqlCommand(query.Replace("\n", "").Replace("\r", ""), connection);

                    if (dicParametros != null)
                    {
                        foreach (KeyValuePair<string, object> parametro in dicParametros)
                        {
                            if (parametro.Value != null)
                                command.Parameters.AddWithValue(parametro.Key, parametro.Value);
                            else
                                command.Parameters.AddWithValue(parametro.Key, DBNull.Value);
                        }
                    }

                    SqlDataAdapter odaSQL = new SqlDataAdapter(command);
                    odaSQL.Fill(dsResultado, "dados");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " : " + ex.StackTrace);
            }

            return dsResultado;
        }

        public long listarTotalLinhas(string table, string where = null, Dictionary<string, object> dicParametros = null)
        {
            long quantidade;

            try
            {
                using (SqlConnection connection = new SqlConnection(Connection.bdConnection))
                {
                    SqlCommand command = new SqlCommand("SELECT count(*) FROM " + table + (string.IsNullOrEmpty(where) ? "" : where), connection);

                    if (dicParametros != null)
                    {
                        foreach (KeyValuePair<string, object> parametro in dicParametros)
                        {
                            if (parametro.Value != null)
                                command.Parameters.AddWithValue(parametro.Key, parametro.Value);
                            else
                                command.Parameters.AddWithValue(parametro.Key, DBNull.Value);
                        }
                    }

                    command.Connection.Open();
                    quantidade = Convert.ToInt32(command.ExecuteScalar());
                    command.Connection.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return quantidade;
        }

        #endregion

        #region " Inserir ou Alterar ou Excluir "

        public void inserirAlterarExcluir(string query, Dictionary<string, object> dicParametros)
        {
            try
            {

                using (SqlConnection connection = new SqlConnection(Connection.bdConnection))
                {
                    SqlCommand command = new SqlCommand(query.Replace("\n", "").Replace("\r", ""), connection);

                    foreach (KeyValuePair<string, object> parametro in dicParametros)
                    {
                        if (parametro.Value != null)
                            command.Parameters.AddWithValue(parametro.Key, parametro.Value);
                        else
                            command.Parameters.AddWithValue(parametro.Key, DBNull.Value);
                    }

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int inserirAlterarExcluirRetornoPK(string query, Dictionary<string, object> dicParametros)
        {
            int retornoPK = new int();
            try
            {
                using (SqlConnection connection = new SqlConnection(Connection.bdConnection))
                {
                    SqlCommand command = new SqlCommand(query.Replace("\n", "").Replace("\r", ""), connection);

                    foreach (KeyValuePair<string, object> parametro in dicParametros)
                    {
                        if (parametro.Value != null)
                            command.Parameters.AddWithValue(parametro.Key, parametro.Value);
                        else
                            command.Parameters.AddWithValue(parametro.Key, DBNull.Value);
                    }

                    connection.Open();
                    retornoPK = (int)command.ExecuteScalar();
                    connection.Close();
                }

                return retornoPK;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region " SqlBulkCopy "

        public void inserirBulkCopy(DataSet dsRetorno, string tableName)
        {
            try
            {
                using (SqlBulkCopy copy = new SqlBulkCopy(Connection.bdConnection))
                {
                    //Remove o tempo de timeout default de 30 segundos
                    copy.BulkCopyTimeout = 0;

                    //Number of rows in each batch. At the end of each batch, the rows in the batch are sent to the server.
                    copy.BatchSize = dsRetorno.Tables[0].Rows.Count;

                    //NotifyAfter
                    copy.NotifyAfter = dsRetorno.Tables[0].Rows.Count;

                    //Nome da tabela
                    copy.DestinationTableName = tableName;

                    foreach (DataColumn column in dsRetorno.Tables[0].Columns)
                    {
                        copy.ColumnMappings.Add(column.ColumnName.ToLower(), column.ColumnName.ToLower());
                    }

                    copy.WriteToServer(dsRetorno.Tables[0]);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable CreateDataTable<T>(IEnumerable<T> list)
        {
            Type type = typeof(T);
            var properties = type.GetProperties();

            DataTable dataTable = new DataTable();
            foreach (PropertyInfo info in properties)
            {
                dataTable.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
            }

            foreach (T entity in list)
            {
                object[] values = new object[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    values[i] = properties[i].GetValue(entity);
                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        #endregion

        #region " Dispose "

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
