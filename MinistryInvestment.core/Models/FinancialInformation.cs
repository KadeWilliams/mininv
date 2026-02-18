using System.ComponentModel.DataAnnotations;

namespace MinistryInvestment.Core.Models;

public class FinancialInformation
{
    public int FinancialInformationID { get; set; }
    public int OrganizationID { get; set; }
    [Display(Name = "Fiscal Year Start"), Required()]
    public DateTime FiscalYear { get; set; }
    [Display(Name = "Fiscal Year End"), Required()]
    public DateTime FiscalYearEnd
    {
        get
        {
            return FiscalYear.AddMonths(11);
        }
    }
    [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
    public decimal Revenue { get; set; }
    public decimal RevenueGrowthPercent { get; set; }
    [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
    public decimal Programs { get; set; }
    public decimal ProgramsPercent { get; set; }
    [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
    public decimal Administration { get; set; }
    public decimal AdministrationPercent { get; set; }
    [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
    public decimal Fundraising { get; set; }
    public decimal FundraisingPercent { get; set; }
    [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
    public decimal Liabilities { get; set; }
    public decimal LiabilitiesPercent { get; set; }
    [Display(Name = "Total Expenses"), DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
    public decimal TotalExpenses { get; set; }
    public decimal TotalExpensesPercent { get; set; }
    [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
    public decimal Cash { get; set; }
    public decimal CashPercent { get; set; }
    public int DocumentID { get; set; }
    public string DocumentName { get; set; } = string.Empty;
    public int FolderID { get; set; }
    public byte[]? FinancialDocument { get; set; }
    public string? Notes { get; set; }
    public string? LastChangeUser { get; set; }
    public DateTime LastChangeDttm { get; set; }
}