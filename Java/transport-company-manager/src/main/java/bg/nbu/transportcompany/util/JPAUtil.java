package bg.nbu.transportcompany.util;

import javax.persistence.EntityManager;
import javax.persistence.EntityManagerFactory;
import javax.persistence.Persistence;
import java.util.HashMap;
import java.util.Map;

public final class JPAUtil {

    private static EntityManagerFactory entityManagerFactory;

    private JPAUtil() {
    }

    public static synchronized EntityManagerFactory getEntityManagerFactory() {
        if (entityManagerFactory == null) {
            String unitName = getPersistenceUnit();
            Map<String, Object> props = new HashMap<>();

            boolean isTest = unitName.toLowerCase().contains("test");
            String urlKey = isTest ? "db.test.url" : "db.url";
            String userKey = isTest ? "db.test.user" : "db.user";
            String passKey = isTest ? "db.test.pass" : "db.pass";

            props.put("javax.persistence.jdbc.url", System.getProperty(urlKey));
            props.put("javax.persistence.jdbc.user", System.getProperty(userKey));
            props.put("javax.persistence.jdbc.password", System.getProperty(passKey));

            props.values().removeIf(v -> v == null);

            entityManagerFactory = Persistence.createEntityManagerFactory(unitName, props);
        }
        return entityManagerFactory;
    }

    public static EntityManager getEntityManager() {
        return getEntityManagerFactory().createEntityManager();
    }

    public static synchronized void shutdown() {
        if (entityManagerFactory != null) {
            entityManagerFactory.close();
            entityManagerFactory = null;
        }
    }

    public static String getPersistenceUnit() {
        return System.getProperty("persistence.unit", "transportPU");
    }
}
