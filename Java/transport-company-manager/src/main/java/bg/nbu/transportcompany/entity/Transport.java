package bg.nbu.transportcompany.entity;

import bg.nbu.transportcompany.validation.ValidTransportAssignment;

import javax.persistence.*;
import javax.validation.constraints.NotBlank;
import javax.validation.constraints.NotNull;
import javax.validation.constraints.Positive;
import javax.validation.constraints.Size;
import java.math.BigDecimal;
import java.time.LocalDate;

@Entity
@Table(name = "transport")
@ValidTransportAssignment
public class Transport {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @NotNull
    @Enumerated(EnumType.STRING)
    @Column(nullable = false, length = 20)
    private TransportType type;

    @NotBlank
    @Size(max = 120)
    @Column(nullable = false, length = 120)
    private String origin;

    @NotBlank
    @Size(max = 120)
    @Column(nullable = false, length = 120)
    private String destination;

    @NotNull
    @Column(name = "departure_date", nullable = false)
    private LocalDate departureDate;

    @NotNull
    @Column(name = "arrival_date", nullable = false)
    private LocalDate arrivalDate;

    @Column(name = "passenger_count")
    private Integer passengerCount;

    @Column(name = "cargo_weight_kg")
    private Double cargoWeightKg;

    @NotNull
    @Positive
    @Column(nullable = false, precision = 12, scale = 2)
    private BigDecimal price;

    @NotNull
    @Enumerated(EnumType.STRING)
    @Column(name = "payment_status", nullable = false, length = 10)
    private PaymentStatus paymentStatus = PaymentStatus.UNPAID;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "company_id", nullable = false)
    private TransportCompany company;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "client_id", nullable = false)
    private Client client;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "driver_id", nullable = false)
    private Employee driver;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "vehicle_id", nullable = false)
    private Vehicle vehicle;

    public Transport() {
    }

    public Transport(TransportType type, String origin, String destination,
                     LocalDate departureDate, LocalDate arrivalDate,
                     Integer passengerCount, Double cargoWeightKg,
                     BigDecimal price, PaymentStatus paymentStatus) {
        this.type = type;
        this.origin = origin;
        this.destination = destination;
        this.departureDate = departureDate;
        this.arrivalDate = arrivalDate;
        this.passengerCount = passengerCount;
        this.cargoWeightKg = cargoWeightKg;
        this.price = price;
        this.paymentStatus = paymentStatus;
    }

    public Long getId() {
        return id;
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

    public TransportCompany getCompany() {
        return company;
    }

    public Client getClient() {
        return client;
    }

    public Employee getDriver() {
        return driver;
    }

    public Vehicle getVehicle() {
        return vehicle;
    }

    public void setId(Long id) {
        this.id = id;
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

    public void setCompany(TransportCompany company) {
        this.company = company;
    }

    public void setClient(Client client) {
        this.client = client;
    }

    public void setDriver(Employee driver) {
        this.driver = driver;
    }

    public void setVehicle(Vehicle vehicle) {
        this.vehicle = vehicle;
    }
}
