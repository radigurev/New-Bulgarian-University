package models.enums;

public enum PaperSize {
    A5(0),
    A4(1),
    A3(2),
    A2(3),
    A1(4);

    private final int stepsUp;
    PaperSize(int stepsUp) { this.stepsUp = stepsUp; }
    public int stepsUp() { return stepsUp; }
}
