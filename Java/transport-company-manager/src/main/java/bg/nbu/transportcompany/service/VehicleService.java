package bg.nbu.transportcompany.service;

import bg.nbu.transportcompany.dto.VehicleDTO;

import java.util.List;

public interface VehicleService {

    VehicleDTO create(VehicleDTO dto);

    List<VehicleDTO> getAll();

    List<VehicleDTO> getAllByCompany(Long companyId);

    VehicleDTO getById(Long id);

    VehicleDTO update(Long id, VehicleDTO dto);

    void delete(Long id);
}
