package bg.nbu.transportcompany.dao.impl;

import bg.nbu.transportcompany.dao.ClientDao;
import bg.nbu.transportcompany.entity.Client;

import javax.persistence.EntityManager;
import java.util.List;
import java.util.Optional;

public class ClientDaoImpl implements ClientDao {

    private final EntityManager em;

    public ClientDaoImpl(EntityManager em) {
        this.em = em;
    }

    @Override
    public Client save(Client client) {
        em.persist(client);
        return client;
    }

    @Override
    public Optional<Client> findById(Long id) {
        return Optional.ofNullable(em.find(Client.class, id));
    }

    @Override
    public List<Client> findAll() {
        return em.createQuery("SELECT c FROM Client c ORDER BY c.name ASC", Client.class)
                .getResultList();
    }

    @Override
    public Client updateById(Long id, String name, String phone, String email) {
        Client c = em.find(Client.class, id);
        if (c == null) {
            return null;
        }
        c.setName(name);
        c.setPhone(phone);
        c.setEmail(email);
        return c;
    }

    @Override
    public void deleteById(Long id) {
        Client c = em.find(Client.class, id);
        if (c != null) {
            em.remove(c);
        }
    }
}
