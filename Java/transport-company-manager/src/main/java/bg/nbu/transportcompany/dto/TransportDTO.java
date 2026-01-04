package bg.nbu.transportcompany.dto;

import bg.nbu.transportcompany.entity.PaymentStatus;
import bg.nbu.transportcompany.entity.TransportType;

import javax.validation.constraints.NotBlank;
import javax.validation.constraints.NotNull;
import javax.validation.constraints.Positive;
import javax.validation.constraints.Size;
import java.math.BigDecimal;
import java.time.LocalDate;

public class TransportDTO {

    private Long id;

    @NotNull
    private Long companyId;

    @NotNull
    private Long clientId;

    @NotNull
    private Long driverId;

    @NotNull
    private Long vehicleId;

    @NotNull
    private TransportType type;

    @NotBlank
    @Size(max = 120)
    private String origin;

    @NotBlank
    @Size(max = 120)
    private String destination;

    @NotNull
    private LocalDate departureDate;

    @NotNull
    private LocalDate arrivalDate;

    private Integer passengerCount;

    private Double cargoWeightKg;

    @NotNull
    @Positive
    private BigDecimal price;

    @NotNull
    private PaymentStatus paymentStatus = PaymentStatus.UNPAID;

    public TransportDTO() {
    }

    public Long getId() {
        return id;
    }

    public Long getCompanyId() {
        return companyId;
    }

    public Long getClientId() {
        return clientId;
    }

    public Long getDriverId() {
        return driverId;
    }

    public Long getVehicleId() {
        return vehicleId;
    }

    public TransportType getType() {
        return type;
    }

    public String getOrigin() {
        return origin;
    }

    public String getDestination() {
        return destination;
    }

    public LocalDate getDepartureDate() {
        return departureDate;
    }

    public LocalDate getArrivalDate() {
        return arrivalDate;
    }

    public Integer getPassengerCount() {
        return passengerCount;
    }

    public Double getCargoWeightKg() {
        return cargoWeightKg;
    }

    public BigDecimal getPrice() {
        return price;
    }

    public PaymentStatus getPaymentStatus() {
        return paymentStatus;
    }

    public void setId(Long id) {
        this.id = id;
    }

    public void setCompanyId(Long companyId) {
        this.companyId = companyId;
    }

    public void setClientId(Long clientId) {
        this.clientId = clientId;
    }

    public void setDriverId(Long driverId) {
        this.driverId = driverId;
    }

    public void setVehicleId(Long vehicleId) {
        this.vehicleId = vehicleId;
    }

    public void setType(TransportType type) {
        this.type = type;
    }

    public void setOrigin(String origin) {
        this.origin = origin;
    }

    public void setDestination(String destination) {
        this.destination = destination;
    }

    public void setDepartureDate(LocalDate departureDate) {
        this.departureDate = departureDate;
    }

    public void setArrivalDate(LocalDate arrivalDate) {
        this.arrivalDate = arrivalDate;
    }

    public void setPassengerCount(Integer passengerCount) {
        this.passengerCount = passengerCount;
    }

    public void setCargoWeightKg(Double cargoWeightKg) {
        this.cargoWeightKg = cargoWeightKg;
    }

    public void setPrice(BigDecimal price) {
        this.price = price;
    }

    public void setPaymentStatus(PaymentStatus paymentStatus) {
        this.paymentStatus = paymentStatus;
    }
}
