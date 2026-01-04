package bg.nbu.transportcompany.service.impl;

import bg.nbu.transportcompany.dao.ClientDao;
import bg.nbu.transportcompany.dao.impl.ClientDaoImpl;
import bg.nbu.transportcompany.dto.ClientDTO;
import bg.nbu.transportcompany.entity.Client;
import bg.nbu.transportcompany.exception.EntityNotFoundException;
import bg.nbu.transportcompany.mapper.DtoMapper;
import bg.nbu.transportcompany.service.ClientService;
import bg.nbu.transportcompany.util.JPAUtil;
import bg.nbu.transportcompany.util.TxUtil;
import bg.nbu.transportcompany.util.ValidationUtil;

import javax.persistence.EntityManager;
import java.util.List;
import java.util.stream.Collectors;

public class ClientServiceImpl implements ClientService {

    @Override
    public ClientDTO create(ClientDTO dto) {
        ValidationUtil.validateOrThrow(dto);
        EntityManager em = JPAUtil.getEntityManager();

        try {
            return TxUtil.doInTransaction(em, e -> {
                ClientDao dao = new ClientDaoImpl(e);
                Client entity = DtoMapper.toEntity(dto);
                dao.save(entity);
                return DtoMapper.toDto(entity);
            });
        }finally {
            em.close();
        }
    }

    @Override
    public List<ClientDTO> getAll() {
        EntityManager em = JPAUtil.getEntityManager();

        try {
            ClientDao dao = new ClientDaoImpl(em);
            return dao.findAll().stream().map(DtoMapper::toDto).collect(Collectors.toList());
        }finally {
            em.close();
        }
    }

    @Override
    public ClientDTO getById(Long id) {
        EntityManager em = JPAUtil.getEntityManager();

        try {
            ClientDao dao = new ClientDaoImpl(em);
            Client c = dao.findById(id).orElseThrow(() -> new EntityNotFoundException("Client not found. id=" + id));
            return DtoMapper.toDto(c);
        }finally {
            em.close();
        }
    }

    @Override
    public ClientDTO update(Long id, ClientDTO dto) {
        ValidationUtil.validateOrThrow(dto);

        EntityManager em = JPAUtil.getEntityManager();
        try {
            return TxUtil.doInTransaction(em, e -> {
                ClientDao dao = new ClientDaoImpl(e);
                Client updated = dao.updateById(id, dto.getName(), dto.getPhone(), dto.getEmail());
                if (updated == null) {
                    throw new EntityNotFoundException("Client not found. id=" + id);
                }
                return DtoMapper.toDto(updated);
            });
        }finally {
            em.close();
        }
    }

    @Override
    public void delete(Long id) {
        EntityManager em = JPAUtil.getEntityManager();

        try {
            TxUtil.doInTransaction(em, e -> {
                ClientDao dao = new ClientDaoImpl(e);
                if (dao.findById(id).isEmpty()) {
                    throw new EntityNotFoundException("Client not found. id=" + id);
                }
                dao.deleteById(id);
                return null;
            });
        }finally {
            em.close();
        }
    }
}
