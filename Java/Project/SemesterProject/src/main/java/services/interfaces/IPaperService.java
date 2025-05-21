package services.interfaces;

import models.enums.PaperSize;
import models.enums.PaperType;

import java.math.BigDecimal;

public interface IPaperService {
    BigDecimal basePrice(PaperType type);
    BigDecimal priceFor(PaperType type, PaperSize size);
}
