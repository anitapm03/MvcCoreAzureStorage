using Azure;
using Azure.Data.Tables;

namespace MvcCoreAzureStorage.Models
{
    public class Cliente : ITableEntity
    {
        public string Nombre { get; set; }
        public int Salario { get; set; }
        public int Edad { get; set; }

        //necesitamos un ID del cliente
        //debemos implementar que el ID
        //y el ROW KEY sean lo mismo.
        private int _IdCliente;
        public int IdCliente
        {
            get { return this._IdCliente; }
            set
            {
                this._IdCliente = value;
                this.RowKey = value.ToString();
            }
        }

        //quiero un campo que represente un 
        //conjunto para los clientes, p ej, empresa
        //pondremos dicho campo como partition key
        private string _Empresa;

        public string Empresa
        {
            get { return this._Empresa; }
            set
            {
                this._Empresa = value;
                this.PartitionKey = value;
            }
        }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
