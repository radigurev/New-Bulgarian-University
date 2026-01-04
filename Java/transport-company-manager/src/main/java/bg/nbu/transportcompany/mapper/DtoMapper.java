package bg.nbu.transportcompany.mapper;

import bg.nbu.transportcompany.dto.*;
import bg.nbu.transportcompany.entity.*;

public final class DtoMapper {

    private DtoMapper() {
    }

    public static TransportCompanyDTO toDto(TransportCompany entity) {
        if (entity == null) {
            return null;
        }
        return new TransportCompanyDTO(entity.getId(), entity.getName(), entity.getAddress());
    }

    public static TransportCompany toEntity(TransportCompanyDTO dto) {
        if (dto == null) {
            return null;
        }
        TransportCompany c = new TransportCompany();
        c.setId(dto.getId());
        c.setName(dto.getName());
        c.setAddress(dto.getAddress());
        return c;
    }

    public static ClientDTO toDto(Client entity) {
        if (entity == null) {
            return null;
        }
        return new ClientDTO(entity.getId(), entity.getName(), entity.getPhone(), entity.getEmail());
    }

    public static Client toEntity(ClientDTO dto) {
        if (dto == null) {
            return null;
        }
        Client c = new Client();
        c.setId(dto.getId());
        c.setName(dto.getName());
        c.setPhone(dto.getPhone());
        c.setEmail(dto.getEmail());
        return c;
    }

    public static EmployeeDTO toDto(Employee entity) {
        if (entity == null) {
            return null;
        }
        EmployeeDTO dto = new EmployeeDTO();
        dto.setId(entity.getId());
        dto.setCompanyId(entity.getCompany() != null ? entity.getCompany().getId() : null);
        dto.setFirstName(entity.getFirstName());
        dto.setLastName(entity.getLastName());
        dto.setRole(entity.getRole());
        dto.setQualification(entity.getQualification());
        dto.setSalary(entity.getSalary());
        return dto;
    }

    public static VehicleDTO toDto(Vehicle entity) {
        if (entity == null) {
            return null;
        }
        VehicleDTO dto = new VehicleDTO();
        dto.setId(entity.getId());
        dto.setCompanyId(entity.getCompany() != null ? entity.getCompany().getId() : null);
        dto.setPlateNumber(entity.getPlateNumber());
        dto.setType(entity.getType());
        dto.setSeatCapacity(entity.getSeatCapacity());
        dto.setMaxLoadKg(entity.getMaxLoadKg());
        return dto;
    }

    public static TransportDTO toDto(Transport entity) {
        if (entity == null) {
            return null;
        }
        TransportDTO dto = new TransportDTO();
        dto.setId(entity.getId());
        dto.setCompanyId(entity.getCompany() != null ? entity.getCompany().getId() : null);
        dto.setClientId(entity.getClient() != null ? entity.getClient().getId() : null);
        dto.setDriverId(entity.getDriver() != null ? entity.getDriver().getId() : null);
        dto.setVehicleId(entity.getVehicle() != null ? entity.getVehicle().getId() : null);
        dto.setType(entity.getType());
        dto.setOrigin(entity.getOrigin());
        dto.setDestination(entity.getDestination());
        dto.setDepartureDate(entity.getDepartureDate());
        dto.setArrivalDate(entity.getArrivalDate());
        dto.setPassengerCount(entity.getPassengerCount());
        dto.setCargoWeightKg(entity.getCargoWeightKg());
        dto.setPrice(entity.getPrice());
        dto.setPaymentStatus(entity.getPaymentStatus());
        return dto;
    }
}
