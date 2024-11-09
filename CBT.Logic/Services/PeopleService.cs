using Microsoft.EntityFrameworkCore;

using CBT.Domain;
using CBT.Domain.Entities;

namespace CBT.Logic.Services
{
    public class PeopleService(CBTDataContextMARS dataContext)
    {
        private readonly CBTDataContext _dataContext = dataContext;

        public async Task CreatePatient(string name, string userId)
        {
            _dataContext.Set<Patient>().Add(new Patient
            {
                DisplayName = name,
                UserId = userId
            });
           
            await _dataContext.SaveChangesAsync();
        }

        public async Task CreatePsychologist(string name, string userId)
        {
            _dataContext.Set<Psychologist>().Add(new Psychologist
            {
                DisplayName = name,
                UserId = userId
            });

            await _dataContext.SaveChangesAsync();
        }

        public async Task<Psychologist?> GetPsychoilogist(string userId)
        {
            return await _dataContext.Set<Psychologist>().AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<Patient?> GetPatient(string userId)
        {
            return await _dataContext.Set<Patient>().AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<PatientPsychologist?> GetExistingConnection(int patientId, int psychologistId)
        {
            var connection = await _dataContext.Set<PatientPsychologist>().FirstOrDefaultAsync(x => x.PatientId == patientId && x.PsychologistId == psychologistId);

            return connection;
        }

        public async Task<bool> Connect(Patient patient, Psychologist psychologist)
        {
            if (patient == null || psychologist == null)
                return false;

            var peopleConnection = await GetExistingConnection(patient.Id, psychologist.Id);
            if (peopleConnection != null)
                return true; // already created

            peopleConnection = new PatientPsychologist { PatientId = patient.Id, PsychologistId = psychologist.Id };
            _dataContext.Set<PatientPsychologist>().Add(peopleConnection);

            await _dataContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<PatientPsychologist>> GetConnectionsFor(int? patientId, int? psychologistId)
        {
            var result = new List<PatientPsychologist>();

            //if (patientId != null && psychologistId != null)
            {
                try
                {
                    result = await _dataContext.Set<PatientPsychologist>()
                        .Include(x => x.Patient)
                        .Include(x => x.Psychologist)
                        .Where(x =>
                            x.PatientId == patientId ||
                            x.PsychologistId == psychologistId)
                        .ToListAsync();
                }
                catch(Exception e)
                {
                    return result;
                }
            }
            //else if (patientId != null)
            //{
            //    result = await _dataContext.Set<PatientPsychologist>()
            //        .Where(x =>
            //            x.PatientId == patientId)
            //        .ToListAsync();
            //}
            //else if (patientId != null)
            //{
            //    result = await _dataContext.Set<PatientPsychologist>()
            //        .Where(x =>
            //            x.PsychologistId == psychologistId)
            //        .ToListAsync();
            //}

            return result;
        }
    }
}
