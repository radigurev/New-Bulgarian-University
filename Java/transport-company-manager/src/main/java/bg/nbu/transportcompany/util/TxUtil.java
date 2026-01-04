package bg.nbu.transportcompany.util;

import bg.nbu.transportcompany.exception.DataAccessException;

import javax.persistence.EntityManager;
import javax.persistence.EntityTransaction;
import java.util.function.Function;

public final class TxUtil {

    private TxUtil() {
    }

    public static <T> T doInTransaction(EntityManager em, Function<EntityManager, T> action) {
        EntityTransaction tx = em.getTransaction();
        try {
            tx.begin();
            T result = action.apply(em);
            tx.commit();
            return result;
        } catch (RuntimeException ex) {
            if (tx.isActive()) {
                tx.rollback();
            }
            throw ex;
        } catch (Exception ex) {
            if (tx.isActive()) {
                tx.rollback();
            }
            throw new DataAccessException("Unexpected data access error.", ex);
        }
    }
}
