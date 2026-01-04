package bg.nbu.transportcompany.report;

import java.math.BigDecimal;

public class DriverRevenueDTO {

    private Long driverId;
    private String driverName;
    private BigDecimal revenue;

    public DriverRevenueDTO(Long driverId, String driverName, BigDecimal revenue) {
        this.driverId = driverId;
        this.driverName = driverName;
        this.revenue = revenue;
    }

    public Long getDriverId() {
        return driverId;
    }

    public String getDriverName() {
        return driverName;
    }

    public BigDecimal getRevenue() {
        return revenue;
    }

    @Override
    public String toString() {
        return "DriverRevenueDTO{" +
                "driverId=" + driverId +
                ", driverName='" + driverName + '\'' +
                ", revenue=" + revenue +
                '}';
    }
}
