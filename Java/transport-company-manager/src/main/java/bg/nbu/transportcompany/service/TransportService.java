package bg.nbu.transportcompany.service;

import bg.nbu.transportcompany.dto.TransportDTO;

import java.util.List;

public interface TransportService {

    TransportDTO create(TransportDTO dto);

    List<TransportDTO> getAll();

    TransportDTO getById(Long id);

    List<TransportDTO> findByDestination(String destinationLike);

    TransportDTO update(Long id, TransportDTO dto);

    void delete(Long id);

    TransportDTO markPaid(Long id);
}
