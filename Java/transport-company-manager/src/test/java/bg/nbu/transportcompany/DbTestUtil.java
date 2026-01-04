package bg.nbu.transportcompany;

import bg.nbu.transportcompany.util.JPAUtil;

import javax.persistence.EntityManager;

public final class DbTestUtil {

    private DbTestUtil() {
    }

    public static void truncateAll() {

        EntityManager em = JPAUtil.getEntityManager();

        try {
            em.getTransaction().begin();

            em.createNativeQuery("SET FOREIGN_KEY_CHECKS = 0").executeUpdate();

//            em.createNativeQuery("TRUNCATE TABLE transport").executeUpdate();
//            em.createNativeQuery("TRUNCATE TABLE vehicle").executeUpdate();
//            em.createNativeQuery("TRUNCATE TABLE employee").executeUpdate();
//            em.createNativeQuery("TRUNCATE TABLE client").executeUpdate();
//            em.createNativeQuery("TRUNCATE TABLE transport_company").executeUpdate();

            em.createNativeQuery("SET FOREIGN_KEY_CHECKS = 1").executeUpdate();
            em.getTransaction().commit();
        }finally {
            em.close();
        }
    }

    public static void truncateTransportsOnly() {
        EntityManager em = JPAUtil.getEntityManager();
        try {
            em.getTransaction().begin();
            em.createNativeQuery("SET FOREIGN_KEY_CHECKS = 0").executeUpdate();
            em.createNativeQuery("TRUNCATE TABLE transport").executeUpdate();
            em.createNativeQuery("SET FOREIGN_KEY_CHECKS = 1").executeUpdate();
            em.getTransaction().commit();
        }finally {
            em.close();
        }
    }
}
