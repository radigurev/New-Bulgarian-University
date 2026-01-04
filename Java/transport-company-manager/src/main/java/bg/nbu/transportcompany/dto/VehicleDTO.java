package bg.nbu.transportcompany.dto;

import bg.nbu.transportcompany.entity.VehicleType;

import javax.validation.constraints.NotBlank;
import javax.validation.constraints.NotNull;
import javax.validation.constraints.Positive;
import javax.validation.constraints.Size;

public class VehicleDTO {

    private Long id;

    @NotNull
    private Long companyId;

    @NotBlank
    @Size(max = 20)
    private String plateNumber;

    @NotNull
    private VehicleType type;

    @Positive
    private int seatCapacity;

    @Positive
    private double maxLoadKg;

    public VehicleDTO() {
    }

    public Long getId() {
        return id;
    }

    public Long getCompanyId() {
        return companyId;
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

    public void setId(Long id) {
        this.id = id;
    }

    public void setCompanyId(Long companyId) {
        this.companyId = companyId;
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
}
