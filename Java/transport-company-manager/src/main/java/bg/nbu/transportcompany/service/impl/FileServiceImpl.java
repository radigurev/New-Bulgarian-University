package bg.nbu.transportcompany.service.impl;

import bg.nbu.transportcompany.dao.TransportDao;
import bg.nbu.transportcompany.dao.impl.TransportDaoImpl;
import bg.nbu.transportcompany.dto.TransportDTO;
import bg.nbu.transportcompany.entity.PaymentStatus;
import bg.nbu.transportcompany.entity.Transport;
import bg.nbu.transportcompany.mapper.DtoMapper;
import bg.nbu.transportcompany.service.FileService;
import bg.nbu.transportcompany.service.TransportService;
import bg.nbu.transportcompany.util.CsvUtil;
import bg.nbu.transportcompany.util.JPAUtil;

import javax.persistence.EntityManager;
import java.nio.file.Path;
import java.time.LocalDate;
import java.util.ArrayList;
import java.util.List;
import java.util.stream.Collectors;

public class FileServiceImpl implements FileService {

    private final TransportService transportService;

    public FileServiceImpl(TransportService transportService) {
        this.transportService = transportService;
    }

    @Override
    public Path exportTransportsToCsv(Path outputFile) {
        List<TransportDTO> transports = transportService.getAll();
        List<String[]> rows = new ArrayList<>();

        rows.add(new String[] {
                "id","companyId","clientId","driverId","vehicleId","type","origin","destination",
                "departureDate","arrivalDate","passengerCount","cargoWeightKg","price","paymentStatus"
        });

        for (TransportDTO t : transports) {
            rows.add(new String[] {
                    safe(t.getId()),
                    safe(t.getCompanyId()),
                    safe(t.getClientId()),
                    safe(t.getDriverId()),
                    safe(t.getVehicleId()),
                    safe(t.getType()),
                    t.getOrigin(),
                    t.getDestination(),
                    safe(t.getDepartureDate()),
                    safe(t.getArrivalDate()),
                    safe(t.getPassengerCount()),
                    safe(t.getCargoWeightKg()),
                    safe(t.getPrice()),
                    safe(t.getPaymentStatus())
            });
        }

        CsvUtil.writeAll(outputFile, rows);
        return outputFile;
    }

    @Override
    public int importTransportsFromCsv(Path inputFile) {
        List<String[]> rows = CsvUtil.readAll(inputFile);
        if (rows.isEmpty()) {
            return 0;
        }

        // skip header if present
        int startIndex = isHeader(rows.get(0)) ? 1 : 0;
        int imported = 0;

        for (int i = startIndex; i < rows.size(); i++) {
            String[] r = rows.get(i);
            if (r.length < 14) {
                continue;
            }

            TransportDTO dto = new TransportDTO();
            dto.setCompanyId(parseLong(r[1]));
            dto.setClientId(parseLong(r[2]));
            dto.setDriverId(parseLong(r[3]));
            dto.setVehicleId(parseLong(r[4]));
            dto.setType(bg.nbu.transportcompany.entity.TransportType.valueOf(r[5]));
            dto.setOrigin(r[6]);
            dto.setDestination(r[7]);
            dto.setDepartureDate(LocalDate.parse(r[8]));
            dto.setArrivalDate(LocalDate.parse(r[9]));
            dto.setPassengerCount(parseIntNullable(r[10]));
            dto.setCargoWeightKg(parseDoubleNullable(r[11]));
            dto.setPrice(new java.math.BigDecimal(r[12]));
            dto.setPaymentStatus(PaymentStatus.valueOf(r[13]));

            transportService.create(dto);
            imported++;
        }

        return imported;
    }

    @Override
    public List<TransportDTO> previewCsv(Path inputFile) {
        List<String[]> rows = CsvUtil.readAll(inputFile);
        if (rows.isEmpty()) {
            return List.of();
        }
        int startIndex = isHeader(rows.get(0)) ? 1 : 0;

        List<TransportDTO> list = new ArrayList<>();
        for (int i = startIndex; i < rows.size(); i++) {
            String[] r = rows.get(i);
            if (r.length < 14) {
                continue;
            }
            TransportDTO dto = new TransportDTO();
            dto.setId(parseLongNullable(r[0]));
            dto.setCompanyId(parseLong(r[1]));
            dto.setClientId(parseLong(r[2]));
            dto.setDriverId(parseLong(r[3]));
            dto.setVehicleId(parseLong(r[4]));
            dto.setType(bg.nbu.transportcompany.entity.TransportType.valueOf(r[5]));
            dto.setOrigin(r[6]);
            dto.setDestination(r[7]);
            dto.setDepartureDate(LocalDate.parse(r[8]));
            dto.setArrivalDate(LocalDate.parse(r[9]));
            dto.setPassengerCount(parseIntNullable(r[10]));
            dto.setCargoWeightKg(parseDoubleNullable(r[11]));
            dto.setPrice(new java.math.BigDecimal(r[12]));
            dto.setPaymentStatus(PaymentStatus.valueOf(r[13]));
            list.add(dto);
        }
        return list;
    }

    private static boolean isHeader(String[] row) {
        return row.length > 0 && "id".equalsIgnoreCase(row[0]);
    }

    private static String safe(Object o) {
        return o == null ? "" : String.valueOf(o);
    }

    private static Long parseLong(String s) {
        return Long.parseLong(s.trim());
    }

    private static Long parseLongNullable(String s) {
        String v = s == null ? "" : s.trim();
        if (v.isEmpty()) return null;
        return Long.parseLong(v);
    }

    private static Integer parseIntNullable(String s) {
        String v = s == null ? "" : s.trim();
        if (v.isEmpty()) return null;
        return Integer.parseInt(v);
    }

    private static Double parseDoubleNullable(String s) {
        String v = s == null ? "" : s.trim();
        if (v.isEmpty()) return null;
        return Double.parseDouble(v);
    }
}
