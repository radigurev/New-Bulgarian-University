package bg.nbu.transportcompany.dto;

import javax.validation.constraints.NotBlank;
import javax.validation.constraints.Size;

public class TransportCompanyDTO {

    private Long id;

    @NotBlank
    @Size(max = 120)
    private String name;

    @Size(max = 255)
    private String address;

    public TransportCompanyDTO() {
    }

    public TransportCompanyDTO(Long id, String name, String address) {
        this.id = id;
        this.name = name;
        this.address = address;
    }

    public Long getId() {
        return id;
    }

    public String getName() {
        return name;
    }

    public String getAddress() {
        return address;
    }

    public void setId(Long id) {
        this.id = id;
    }

    public void setName(String name) {
        this.name = name;
    }

    public void setAddress(String address) {
        this.address = address;
    }
}
