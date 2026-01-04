package bg.nbu.transportcompany.report;

public class DriverTransportCountDTO {

    private Long driverId;
    private String driverName;
    private long transportCount;

    public DriverTransportCountDTO(Long driverId, String driverName, long transportCount) {
        this.driverId = driverId;
        this.driverName = driverName;
        this.transportCount = transportCount;
    }

    public Long getDriverId() {
        return driverId;
    }

    public String getDriverName() {
        return driverName;
    }

    public long getTransportCount() {
        return transportCount;
    }

    @Override
    public String toString() {
        return "DriverTransportCountDTO{" +
                "driverId=" + driverId +
                ", driverName='" + driverName + '\'' +
                ", transportCount=" + transportCount +
                '}';
    }
}
