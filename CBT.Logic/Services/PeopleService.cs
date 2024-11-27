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

        public async Task<Psychologist?> GetPsychologist(string userId)
        {
            return await _dataContext.Set<Psychologist>().AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<Psychologist?> GetPsychologist(int psychologistId)
        {
            return await _dataContext.Set<Psychologist>().AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == psychologistId);
        }

        public async Task<Patient?> GetPatient(string userId)
        {
            return await _dataContext.Set<Patient>().AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<Patient?> GetPatient(int patientId)
        {
            return await _dataContext.Set<Patient>().AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == patientId);
        }

        public async Task<PatientPsychologist?> GetExistingConnection(int patientId, int psychologistId)
        {
            var connection = await _dataContext.Set<PatientPsychologist>()
                .FirstOrDefaultAsync(x => x.PatientId == patientId && x.PsychologistId == psychologistId);

            return connection;
        }

        public async Task<bool> Connect(Patient? patient, Psychologist? psychologist, bool enable = true)
        {
            if (patient == null || psychologist == null)
                return false;

            var peopleConnection = await GetExistingConnection(patient.Id, psychologist.Id);
            if (peopleConnection != null)// already created
            {
                peopleConnection.Enabled = enable;
            }
            else
            {
                peopleConnection = new PatientPsychologist
                {
                    PatientId = patient.Id,
                    PsychologistId = psychologist.Id,
                    Enabled = enable
                };
                _dataContext.Set<PatientPsychologist>().Add(peopleConnection);
            }

            await _dataContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteConnection(Patient? patient, Psychologist? psychologist)
        {
            if (patient == null || psychologist == null)
                return false;

            var peopleConnection = await GetExistingConnection(patient.Id, psychologist.Id);

            if (peopleConnection == null)
                return false;

            try
            {
                _dataContext.Set<PatientPsychologist>().Remove(peopleConnection);

                await _dataContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<PatientPsychologist>> GetConnectionsFor(int? patientId, int? psychologistId)
        {
            List<PatientPsychologist> result;

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
            catch (Exception)
            {
                return [];
            }

            return result;
        }
    }
}
