package bg.nbu.transportcompany.dao;

import bg.nbu.transportcompany.entity.Client;

import java.util.List;
import java.util.Optional;

public interface ClientDao {

    Client save(Client client);

    Optional<Client> findById(Long id);

    List<Client> findAll();

    Client updateById(Long id, String name, String phone, String email);

    void deleteById(Long id);
}
