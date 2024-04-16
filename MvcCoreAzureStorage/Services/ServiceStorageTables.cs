using Azure.Data.Tables;
using MvcCoreAzureStorage.Models;

namespace MvcCoreAzureStorage.Services
{
    public class ServiceStorageTables
    {
        private TableClient tableClient;

        public ServiceStorageTables(TableServiceClient tableService)
        {
            //inyectamos el service para crear la
            //tabla en caso de que no exista
            this.tableClient = 
                tableService.GetTableClient("clientes");

            //estamos en un constructor y dicho metodo 
            //de obj no puede ser asíncrono
            Task.Run(async () =>
            {
                await
                this.tableClient.CreateIfNotExistsAsync();
            });
        }

        //METODO PARA CREAR REGISTRO DE CLIENTE
        public async Task CreateClientAsync
            (int id, string nombre, int salario, 
            int edad, string empresa)
        {
            Cliente cliente = new Cliente();
            cliente.IdCliente = id;
            cliente.Nombre = nombre;
            cliente.Salario = salario;
            cliente.Edad = edad;
            cliente.Empresa = empresa;
            await this.tableClient.AddEntityAsync<Cliente>(cliente);
        }

        //metodo para buscar clientes por su pk
        //cuando hablamos de este tipo de busquedas
        //dentro de AS Tables, debemos buscar por los
        //datos combinados (Row key y Partition Key)
        public async Task<Cliente> FindClientAsync
            (string partitionKey, string rowKey)
        {
            Cliente cliente = await
                this.tableClient.GetEntityAsync<Cliente>
                (partitionKey, rowKey);
            return cliente;
        }

        //metodo para eliminar registros 
        //para eliminar un registro unico, debemos
        //enviar Row Key y Partition Key
        public async Task DeleteClientAsync
            (string partitionKey, string rowKey)
        {
            await this.tableClient.DeleteEntityAsync
                (partitionKey, rowKey);
        }

        //metodo para recuperar todos los registros
        public async Task<List<Cliente>>
            GetClientesAsync()
        {
            //para poder recuperar datos, aunque sean todos
            //es necesario indicar un query con un filter
            List<Cliente> clientes = new List<Cliente>();
            var query =
                this.tableClient.QueryAsync<Cliente>(filter: "");

            await foreach (var item in query)
            {
                clientes.Add(item);
            }

            return clientes;
        }

        public async Task<List<Cliente>>
            GetClientesEmpresaAsync(string empresa)
        {
            //para filtrar podemos usar la sintaxis 
            //"pura" de tables (eq equals / gt greater than / lt lower than
            //string filtro = "Campo eq valor and Campo2 gt value 2";
            string filtro = "Empresa eq " + empresa;
            var query = 
                this.tableClient.Query<Cliente>
                (x => x.Empresa == empresa);
            return query.ToList();
        }
    }
}
