package bg.nbu.transportcompany;

import bg.nbu.transportcompany.util.JPAUtil;
import org.junit.jupiter.api.AfterAll;
import org.junit.jupiter.api.BeforeAll;

import javax.persistence.EntityManager;
import java.util.Optional;

public abstract class BaseIntegrationTest {

    @BeforeAll
    static void beforeAll() {
        System.setProperty("persistence.unit", "transportTestPU");
        JPAUtil.shutdown();

        setIfMissing("db.test.url", Optional.ofNullable(System.getenv("DB_TEST_URL"))
                .orElse("jdbc:mysql://localhost:3306/transport_company_test?createDatabaseIfNotExist=true&useSSL=false&allowPublicKeyRetrieval=true&serverTimezone=UTC"));
        setIfMissing("db.test.user", Optional.ofNullable(System.getenv("DB_TEST_USER")).orElse("root"));
        setIfMissing("db.test.pass", Optional.ofNullable(System.getenv("DB_TEST_PASS")).orElse("password"));

        EntityManager em = JPAUtil.getEntityManager();
        try {
            em.getTransaction().begin();
            em.getTransaction().commit();
        }finally {
            em.close();
        }
    }

    @AfterAll
    static void afterAll() {
        JPAUtil.shutdown();
    }

    protected static void setIfMissing(String key, String value) {
        if (System.getProperty(key) == null) {
            System.setProperty(key, value);
        }
    }
}
