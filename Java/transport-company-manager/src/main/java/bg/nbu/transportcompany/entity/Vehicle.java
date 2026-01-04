package bg.nbu.transportcompany.entity;

import javax.persistence.*;
import javax.validation.constraints.NotBlank;
import javax.validation.constraints.NotNull;
import javax.validation.constraints.Positive;
import javax.validation.constraints.Size;

@Entity
@Table(name = "vehicle",
       uniqueConstraints = {@UniqueConstraint(name = "uk_vehicle_plate", columnNames = "plate_number")})
public class Vehicle {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @NotBlank
    @Size(max = 20)
    @Column(name = "plate_number", nullable = false, length = 20)
    private String plateNumber;

    @NotNull
    @Enumerated(EnumType.STRING)
    @Column(nullable = false, length = 30)
    private VehicleType type;

    @Positive
    @Column(name = "seat_capacity", nullable = false)
    private int seatCapacity;

    @Positive
    @Column(name = "max_load_kg", nullable = false)
    private double maxLoadKg;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "company_id", nullable = false)
    private TransportCompany company;

    public Vehicle() {
    }

    public Vehicle(String plateNumber, VehicleType type, int seatCapacity, double maxLoadKg) {
        this.plateNumber = plateNumber;
        this.type = type;
        this.seatCapacity = seatCapacity;
        this.maxLoadKg = maxLoadKg;
    }

    public Long getId() {
        return id;
    }

    public String getPlateNumber() {
        return plateNumber;
    }

    public VehicleType getType() {
        return type;
    }

    public int getSeatCapacity() {
        return seatCapacity;
    }

    public double getMaxLoadKg() {
        return maxLoadKg;
    }

    public TransportCompany getCompany() {
        return company;
    }

    public void setId(Long id) {
        this.id = id;
    }

    public void setPlateNumber(String plateNumber) {
        this.plateNumber = plateNumber;
    }

    public void setType(VehicleType type) {
        this.type = type;
    }

    public void setSeatCapacity(int seatCapacity) {
        this.seatCapacity = seatCapacity;
    }

    public void setMaxLoadKg(double maxLoadKg) {
        this.maxLoadKg = maxLoadKg;
    }

    public void setCompany(TransportCompany company) {
        this.company = company;
    }
}
