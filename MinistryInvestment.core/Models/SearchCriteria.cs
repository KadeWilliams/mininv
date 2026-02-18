using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;

namespace MinistryInvestment.Core.Models
{
    public class SearchCriteria
    {
        //Search data
        public string[] criteria { get; set; }
        public string[] OrganizationName { get; set; }
        public string[] ContactName { get; set; }
        public string? Date { get; set; }
        //parsed search datasdf
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        [Display(Name = "Donation Amount")]
        public string? DonationAmountText { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true), Range(0, int.MaxValue)]
        public int? LowerDonationAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true), Range(0, int.MaxValue)]
        public int? UpperDonationAmount { get; set; }
        public int SearchType { get; set; }
        public DataTable SearchOrganizationNamesTable { get; set; }
        public DataTable SearchContactNamesTable { get; set; }
        public DataTable SearchCategoriesTable { get; set; }
        public DataTable SearchCountryCDsTable { get; set; }
        public DataTable SearchPartnerTypesTable { get; set; }
        public DataTable SearchProjectTypesTable { get; set; }
        public DataTable SearchRequestStatusesTable { get; set; }
        public DataTable SearchVoteTeamTable { get; set; }
        public DataTable SearchOrganizationRegionsTable { get; set; }
        public DataTable SearchContactTypesIDTable { get; set; }
        public DataTable SearchFlagsTable { get; set; }

        public SearchCriteria()
        {
            SearchType = 0;
            SearchCategoriesTable = new DataTable("UserCategoriesNameTable");
            SearchCategoriesTable.Columns.Add("CategoryID", typeof(int));

            SearchPartnerTypesTable = new DataTable("UserPartnerTypesTable");
            SearchPartnerTypesTable.Columns.Add("PartnerTypeID", typeof(int));

            SearchRequestStatusesTable = new DataTable("UserRequestStatusesTable");
            SearchRequestStatusesTable.Columns.Add("RequestStatusID", typeof(int));

            SearchVoteTeamTable = new DataTable("UserVoteTeamTable");
            SearchVoteTeamTable.Columns.Add("VoteTeamID", typeof(int));

            SearchOrganizationRegionsTable = new DataTable("UserOrganizationRegionsTable");
            SearchOrganizationRegionsTable.Columns.Add("RegionID", typeof(int));

            SearchContactTypesIDTable = new DataTable("UserContactTypesIDTable");
            SearchContactTypesIDTable.Columns.Add("ContactTypeID", typeof(int));

            SearchProjectTypesTable = new DataTable("UserProjectTypesTable");
            SearchProjectTypesTable.Columns.Add("ProjectTypeID", typeof(int));

            SearchCountryCDsTable = new DataTable("UserCountryCDsTable");
            SearchCountryCDsTable.Columns.Add("CountryID", typeof(int));

            SearchOrganizationNamesTable = new DataTable("UserOrganizationNameTable");
            SearchOrganizationNamesTable.Columns.Add("OrganizationName", typeof(string));

            SearchContactNamesTable = new DataTable("UserContactNamesTable");
            SearchContactNamesTable.Columns.Add("ContactName", typeof(string));

            SearchFlagsTable = new DataTable("UserFlagsTable");
            SearchFlagsTable.Columns.Add("Flag", typeof(string));
        }
        public void ToDataTables()
        {
            if (criteria != null)
            {
                foreach (var item in criteria)
                {
                    if (item == null) break;
                    string[] token = item.Split(',');
                    switch (token[0])
                    {
                        case "Category":
                            DataRow Category = SearchCategoriesTable.NewRow();
                            Category["CategoryID"] = Int32.Parse(token[1]);
                            SearchCategoriesTable.Rows.Add(Category);
                            break;
                        case "Region":
                            DataRow region = SearchOrganizationRegionsTable.NewRow();
                            region["RegionID"] = Int32.Parse(token[1]);
                            SearchOrganizationRegionsTable.Rows.Add(region);
                            break;
                        case "PartnerType":
                            DataRow PartnerType = SearchPartnerTypesTable.NewRow();
                            PartnerType["PartnerTypeID"] = Int32.Parse(token[1]);
                            SearchPartnerTypesTable.Rows.Add(PartnerType);
                            break;
                        case "Country":
                            DataRow Country = SearchCountryCDsTable.NewRow();
                            Country["CountryID"] = Int32.Parse(token[1]);
                            SearchCountryCDsTable.Rows.Add(Country);
                            break;
                        case "Status":
                            DataRow status = SearchRequestStatusesTable.NewRow();
                            status["RequestStatusID"] = Int32.Parse(token[1]);
                            SearchRequestStatusesTable.Rows.Add(status);
                            break;
                        case "Team":
                            DataRow team = SearchVoteTeamTable.NewRow();
                            team["VoteTeamID"] = Int32.Parse(token[1]);
                            SearchVoteTeamTable.Rows.Add(team);
                            break;
                        case "ContactTypeID":
                            DataRow ContactType = SearchContactTypesIDTable.NewRow();
                            ContactType["ContactTypeID"] = Int32.Parse(token[1]);
                            SearchContactTypesIDTable.Rows.Add(ContactType);
                            break;
                        case "ProjectTypeID":
                            DataRow ProjectType = SearchProjectTypesTable.NewRow();
                            ProjectType["ProjectTypeID"] = Int32.Parse(token[1]);
                            SearchProjectTypesTable.Rows.Add(ProjectType);
                            break;
                        case "Flag":
                            DataRow Flag = SearchFlagsTable.NewRow();
                            Flag["Flag"] = token[1];
                            SearchFlagsTable.Rows.Add(Flag);
                            break;
                    }
                }
            }
            if (OrganizationName != null)
            {
                foreach (var item in OrganizationName)
                {
                    DataRow row = SearchOrganizationNamesTable.NewRow();
                    row["OrganizationName"] = "%" + item + "%";
                    SearchOrganizationNamesTable.Rows.Add(row);
                }
            }
            if (ContactName != null)
            {
                foreach (var item in ContactName)
                {
                    DataRow row = SearchContactNamesTable.NewRow();
                    row["ContactName"] = "%" + item + "%";
                    SearchContactNamesTable.Rows.Add(row);
                }
            }
        }
        public void parseDates()
        {
            if (Date != null)
            {
                string[] token = Date.Split("to");
                BeginDate = DateTime.ParseExact(token[0].Trim(), "MM/yyyy", CultureInfo.InvariantCulture);
                if (token.Length > 1)
                {
                    EndDate = DateTime.ParseExact(token[1].Trim(), "MM/yyyy", CultureInfo.InvariantCulture);
                }
            }
        }
    }
}