namespace BHYT.API.Utilities
{
    public class CalculateInsuranceFees
    {
        // Hàm tính toán yếu tố tuổi tác
        private static decimal CalculateAgeFactor(int age)
        {
            if (age <= 20)
            {
                return 1.0m;
            }
            else if (age <= 30)
            {
                return 1.2m;
            }
            else if (age <= 40)
            {
                return 1.4m;
            }
            else if (age <= 50)
            {
                return 1.6m;
            }
            else if (age <= 60)
            {
                return 1.8m;
            }
            else
            {
                return 2.0m;
            }
        }

        // Hàm tính toán yếu tố giới tính
        private static decimal CalculateGenderFactor(string gender)
        {
            if (gender == "Nam")
            {
                return 1.2m;
            }
            else
            {
                return 1.0m;
            }
        }

        // Hàm tính toán yếu tố tình trạng sức khỏe
        private static decimal CalculateHealthFactor(string healthStatus)
        {
            if (healthStatus == "Khỏe mạnh")
            {
                return 1.0m;
            }
            else if (healthStatus == "Bình thường")
            {
                return 1.2m;
            }
            else if (healthStatus == "Có vấn đề sức khỏe")
            {
                return 1.4m;
            }
            else
            {
                return 1.6m;
            }
        }

        // Hàm tính toán yếu tố hút thuốc
        private static decimal CalculateSmokingFactor(bool smoking)
        {
            if (smoking)
            {
                return 1.5m;
            }
            else
            {
                return 1.0m;
            }
        }

        public static decimal CalculateInsurancePremium(int age, string gender, string healthStatus, bool smoking, int coverageAmount, int coverageTerm)
        {
            // Khởi tạo các biến
            decimal basePremium = 1000000;
            decimal ageFactor = 1.0m;
            decimal genderFactor = 1.0m;
            decimal healthFactor = 1.0m;
            decimal smokingFactor = 1.0m;

            // Tính toán các yếu tố
            ageFactor = CalculateAgeFactor(age);
            genderFactor = CalculateGenderFactor(gender);
            healthFactor = CalculateHealthFactor(healthStatus);
            smokingFactor = CalculateSmokingFactor(smoking);

            // Tính toán phí bảo hiểm
            decimal premium = basePremium * ageFactor * genderFactor * healthFactor * smokingFactor;

            // Tính phí theo tháng hoặc năm
            if (coverageTerm == 1)
            {
                return premium / 12;
            }
            else
            {
                return premium;
            }
        }
    }
}
