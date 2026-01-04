package bg.nbu.transportcompany;

import bg.nbu.transportcompany.service.*;
import bg.nbu.transportcompany.service.impl.*;
import bg.nbu.transportcompany.ui.ConsoleMenu;
import bg.nbu.transportcompany.util.JPAUtil;

import javax.persistence.EntityManager;

public class App {

    public static void main(String[] args) {
        CompanyService companyService = new CompanyServiceImpl();
        ClientService clientService = new ClientServiceImpl();
        EmployeeService employeeService = new EmployeeServiceImpl();
        VehicleService vehicleService = new VehicleServiceImpl();
        TransportService transportService = new TransportServiceImpl();
        ReportService reportService = new ReportServiceImpl();
        FileService fileService = new FileServiceImpl(transportService);

        EntityManager em = null;
        try {
            em = JPAUtil.getEntityManager();
        } finally {
            if (em != null && em.isOpen()) {
                em.close();
            }
        }

        ConsoleMenu menu = new ConsoleMenu(companyService, clientService, employeeService, vehicleService, transportService, reportService, fileService);
        menu.run();

        JPAUtil.shutdown();
        System.out.println("Bye!");
    }
}
