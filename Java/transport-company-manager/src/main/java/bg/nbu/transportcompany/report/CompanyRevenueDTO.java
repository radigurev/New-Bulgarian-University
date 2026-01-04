package bg.nbu.transportcompany.report;

import java.math.BigDecimal;

public class CompanyRevenueDTO {

    private Long companyId;
    private String companyName;
    private BigDecimal revenue;

    public CompanyRevenueDTO(Long companyId, String companyName, BigDecimal revenue) {
        this.companyId = companyId;
        this.companyName = companyName;
        this.revenue = revenue;
    }

    public Long getCompanyId() {
        return companyId;
    }

    public String getCompanyName() {
        return companyName;
    }

    public BigDecimal getRevenue() {
        return revenue;
    }

    @Override
    public String toString() {
        return "CompanyRevenueDTO{" +
                "companyId=" + companyId +
                ", companyName='" + companyName + '\'' +
                ", revenue=" + revenue +
                '}';
    }
}
