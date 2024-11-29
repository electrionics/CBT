using Microsoft.EntityFrameworkCore;

using CBT.Domain;
using CBT.Domain.Entities;
using CBT.SharedComponents.Blazor.Model.Reports;

namespace CBT.SharedComponents.Blazor.Services
{
    public class IndicatorsFacade(CBTDataContext dataContext)
    {
        private readonly CBTDataContext _dataContext = dataContext;

        private const string DemoUserId = "DemoClient";


        #region GetProductivityReport

        public async Task<List<ProductivityReportItem>> GetProductivityReport(string? userId)
        {
            var data = await GetReportQuery(userId)
                .ToListAsync();

            return data
                .Select(x => new ProductivityReportItem
                {
                    Date = x.Key,
                    ProductivityValue = x.Sum(y => y.ActualEffort ?? 0)
                })
                .OrderBy(x => x.Date)
                .ToList();
        }

        #endregion

        #region GetWillPowerReport

        public async Task<List<WillReportItem>> GetWillPowerReport(string? userId)
        {
            var data = await GetReportQuery(userId)
                .ToListAsync();

            return data
                .Select(x => new WillReportItem
                {
                    Date = x.Key,
                    WillValue = x.Where(y => y.SupposedEffort != null && y.SupposedPleasure != null)
                        .Sum(y => Math.Max(0, y.SupposedEffort!.Value - y.SupposedPleasure!.Value))
                })
                .OrderBy(x => x.Date)
                .ToList();
        }

        #endregion

        #region GetMotivationChangeReport

        public async Task<List<MotivationChangeReportItem>> GetMotivationChangeReport(string? userId)
        {
            var data = await GetReportQuery(userId)
                .ToListAsync();

            return data
                .Select(x => new MotivationChangeReportItem
                {
                    Date = x.Key,
                    ChangeValue = x.Where(y => 
                            y.SupposedEffort != null &&
                            y.SupposedPleasure != null &&
                            y.ActualEffort != null &&
                            y.ActualPleasure != null)
                        .Sum(y => 
                            y.SupposedEffort!.Value - y.SupposedPleasure!.Value + 
                            y.ActualPleasure!.Value - y.ActualEffort!.Value)
                })
                .OrderBy(x => x.Date)
                .ToList();
        }

        #endregion

        #region GetEstimationRealismReport

        public async Task<List<RealismReportItem>> GetEstimationRealismReport(string? userId) 
        {
            var data = await GetReportQuery(userId)
                .ToListAsync();

            return data
                .Select(x => new RealismReportItem
                {
                    Date = x.Key,
                    RealismValue = Convert.ToInt32(Math.Round((double)200/(
                        Math.Max(1, x.Where(y => y.SupposedEffort != null && y.ActualEffort != null)
                            .Sum(y => Math.Abs(y.SupposedEffort!.Value - y.ActualEffort!.Value))) + 
                        Math.Max(1, x.Where(y => y.SupposedPleasure != null && y.ActualPleasure != null)
                            .Sum(y => Math.Abs(y.SupposedPleasure!.Value - y.ActualPleasure!.Value))
                        ))))
                })
                .ToList();
        }

        #endregion

        #region GetDisciplineReport

        public async Task<List<DisciplineReportItem>> GetDisciplineReport(string? userId)
        {
            var data = await GetReportQuery(userId)
                .ToListAsync();

            return data
                .Select(x => new DisciplineReportItem
                {
                    Date = x.Key,
                    DisciplinePercent = Convert.ToInt32(Math.Round((double)
                        x.Count(y => y.ActualEffort != null && y.ActualPleasure != null) /
                        x.Count()))
                })
                .OrderBy(x => x.Date)
                .ToList();
        }

        #endregion


        #region Helper Methods

        private IQueryable<IGrouping<DateTime, AntiProcrastinationRecord>> GetReportQuery(string? userId)
        {
            return _dataContext.Set<AntiProcrastinationRecord>().AsNoTracking()
                .Where(x => x.Patient.UserId == (userId ?? DemoUserId) && x.PlanDate != null)
                .GroupBy(x => x.PlanDate!.Value);
        }

        #endregion
    }
}
