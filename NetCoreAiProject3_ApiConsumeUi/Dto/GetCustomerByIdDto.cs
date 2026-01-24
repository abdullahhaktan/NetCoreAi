namespace NetCoreAi.Project2_ApiConsumeUi.Dto
{
    public class GetCustomerByIdDto
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerSurname { get; set; }
        public decimal CustomerBalance { get; set; }
    }
}
