package bg.nbu.transportcompany.service;

import bg.nbu.transportcompany.dto.TransportDTO;

import java.nio.file.Path;
import java.util.List;

public interface FileService {

    Path exportTransportsToCsv(Path outputFile);

    int importTransportsFromCsv(Path inputFile);

    List<TransportDTO> previewCsv(Path inputFile);
}
