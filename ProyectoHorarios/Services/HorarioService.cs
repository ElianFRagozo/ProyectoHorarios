using MongoDB.Bson;
using MongoDB.Driver;
using ProyectoHorarios.Models.ProyectoApi.Models;
using ProyectoHorarios.Services.ProyectoHorario.Services;

namespace ProyectoHorarios.Services
{
    public class HorarioService
    {
        private readonly IMongoCollection<Horario> _horario;

        public HorarioService(IMongoDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _horario = database.GetCollection<Horario>("Horario");


        }

        public async Task<List<Horario>> GetHorariosAsync(string idMedico, DateTime dia)
        {
            return await _horario.Find(h => h.IdMedico == idMedico && h.Dia == dia).ToListAsync();
        }
        public async Task<List<Horario>> GetHorariosAsync()
        {
            return await _horario.Find(horario => true).ToListAsync();
        }


        public async Task CreateHorarioAsync(Horario horario)
        {
            var newId = ObjectId.GenerateNewId();
            horario.Id = newId.ToString();

            await _horario.InsertOneAsync(horario);
        }

        public async Task<Horario> GetHorarioIDAsync(string _horarioId)
        {
            var objectId = ObjectId.Parse(_horarioId);
            return await _horario.Find(horario => (horario.Id == objectId.ToString())).FirstOrDefaultAsync();
        }

        public async Task UpdateHorarioAsync(Horario horario)
        {
            var filter = Builders<Horario>.Filter.Eq(h => h.Id, horario.Id);
            await _horario.UpdateOneAsync(filter, Builders<Horario>.Update.Set(h => h.IdMedico, horario.IdMedico)
                .Set(h => h.Dia, horario.Dia)
                .Set(h => h.Hora, horario.Hora));
        }

        public async Task DeleteHorarioAsync(Horario horario)
        {
            var filter = Builders<Horario>.Filter.Eq(h => h.Id, horario.Id);
            await _horario.DeleteOneAsync(filter);
        }
    }

    namespace ProyectoHorario.Services
    {
        public interface IMongoDatabaseSettings
        {
            string ConnectionString { get; set; }
            string DatabaseName { get; set; }
            string NotesCollectionName { get; set; }
        }

        public class MongoDatabaseSettings : IMongoDatabaseSettings
        {
            public string ConnectionString { get; set; }
            public string DatabaseName { get; set; }
            public string NotesCollectionName { get; set; }
        }
    }
}
