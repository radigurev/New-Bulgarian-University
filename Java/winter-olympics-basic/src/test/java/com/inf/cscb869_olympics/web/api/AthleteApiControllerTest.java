package com.inf.cscb869_olympics.web.api;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.inf.cscb869_olympics.config.SecurityConfig;
import com.inf.cscb869_olympics.data.entity.Gender;
import com.inf.cscb869_olympics.dto.AthleteDTO;
import com.inf.cscb869_olympics.dto.CreateAthleteDTO;
import com.inf.cscb869_olympics.exception.AthleteNotFoundException;
import com.inf.cscb869_olympics.service.AthleteService;
import com.inf.cscb869_olympics.service.UserService;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.autoconfigure.web.servlet.WebMvcTest;
import org.springframework.boot.test.mock.mockito.MockBean;
import org.springframework.context.annotation.Import;
import org.springframework.http.MediaType;
import org.springframework.security.test.context.support.WithMockUser;
import org.springframework.test.web.servlet.MockMvc;

import java.time.LocalDate;
import java.util.List;

import static org.hamcrest.Matchers.is;
import static org.mockito.BDDMockito.given;
import static org.mockito.Mockito.willThrow;
import static org.springframework.security.test.web.servlet.request.SecurityMockMvcRequestPostProcessors.csrf;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.delete;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.get;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.post;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.jsonPath;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.status;

@WebMvcTest(AthleteApiController.class)
@Import(SecurityConfig.class)
class AthleteApiControllerTest {

    @Autowired
    private MockMvc mockMvc;

    @Autowired
    private ObjectMapper objectMapper;

    @MockBean
    private AthleteService athleteService;

    @MockBean
    private UserService userService;

    @Test
    void list_returnsAllAthletes_forAnonymous() throws Exception {
        given(athleteService.getAthletes()).willReturn(List.of(
                new AthleteDTO(1L, "Hanna", "Lindholm", "Finland", Gender.FEMALE, LocalDate.of(1998, 4, 12))
        ));

        mockMvc.perform(get("/api/athletes"))
                .andExpect(status().isOk())
                .andExpect(jsonPath("$[0].firstName", is("Hanna")));
    }

    @Test
    void get_returnsNotFound_whenAthleteMissing() throws Exception {
        given(athleteService.getAthlete(99L)).willThrow(new AthleteNotFoundException(99L));

        mockMvc.perform(get("/api/athletes/99"))
                .andExpect(status().isNotFound());
    }

    @Test
    @WithMockUser(authorities = "admin")
    void create_returnsCreated_forAdmin() throws Exception {
        CreateAthleteDTO dto = new CreateAthleteDTO("Anna", "Test", "Bulgaria", Gender.FEMALE, LocalDate.of(2000, 1, 1));
        given(athleteService.createAthlete(org.mockito.ArgumentMatchers.any()))
                .willReturn(new AthleteDTO(5L, "Anna", "Test", "Bulgaria", Gender.FEMALE, LocalDate.of(2000, 1, 1)));

        mockMvc.perform(post("/api/athletes")
                        .with(csrf())
                        .contentType(MediaType.APPLICATION_JSON)
                        .content(objectMapper.writeValueAsString(dto)))
                .andExpect(status().isCreated())
                .andExpect(jsonPath("$.id", is(5)));
    }

    @Test
    @WithMockUser(authorities = "athlete")
    void create_returnsForbidden_forNonAdmin() throws Exception {
        CreateAthleteDTO dto = new CreateAthleteDTO("Anna", "Test", "Bulgaria", Gender.FEMALE, LocalDate.of(2000, 1, 1));

        mockMvc.perform(post("/api/athletes")
                        .with(csrf())
                        .contentType(MediaType.APPLICATION_JSON)
                        .content(objectMapper.writeValueAsString(dto)))
                .andExpect(status().isForbidden());
    }

    @Test
    @WithMockUser(authorities = "admin")
    void delete_returnsNoContent_forAdmin() throws Exception {
        mockMvc.perform(delete("/api/athletes/3").with(csrf()))
                .andExpect(status().isNoContent());
    }
}
