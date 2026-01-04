package bg.nbu.transportcompany.service;

import bg.nbu.transportcompany.dto.ClientDTO;

import java.util.List;

public interface ClientService {

    ClientDTO create(ClientDTO dto);

    List<ClientDTO> getAll();

    ClientDTO getById(Long id);

    ClientDTO update(Long id, ClientDTO dto);

    void delete(Long id);
}
