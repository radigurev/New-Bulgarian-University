package bg.nbu.transportcompany;

import bg.nbu.transportcompany.dto.TransportCompanyDTO;
import bg.nbu.transportcompany.service.CompanyService;
import bg.nbu.transportcompany.service.impl.CompanyServiceImpl;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;

import java.util.List;

import static org.junit.jupiter.api.Assertions.*;

public class CompanyCrudTest extends BaseIntegrationTest {

    private final CompanyService companyService = new CompanyServiceImpl();

    @BeforeEach
    void setup() {
        DbTestUtil.truncateAll();
    }

    @Test
    void companyCrud_works() {
        TransportCompanyDTO created = companyService.create(TestData.company("A"));
        assertNotNull(created.getId());

        TransportCompanyDTO byId = companyService.getById(created.getId());
        assertEquals("A", byId.getName());

        List<TransportCompanyDTO> all = companyService.getAll();
        assertEquals(1, all.size());

        TransportCompanyDTO updDto = new TransportCompanyDTO();
        updDto.setName("A2");
        updDto.setAddress("Sofia2");
        TransportCompanyDTO updated = companyService.update(created.getId(), updDto);
        assertEquals("A2", updated.getName());

        companyService.delete(created.getId());
        assertTrue(companyService.getAll().isEmpty());
    }
}
