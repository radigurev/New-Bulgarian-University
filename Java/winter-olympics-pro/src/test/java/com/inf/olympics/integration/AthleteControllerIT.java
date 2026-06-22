package com.inf.olympics.integration;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.inf.olympics.application.dto.CreateAthleteRequest;
import com.inf.olympics.domain.model.Gender;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.autoconfigure.web.servlet.AutoConfigureMockMvc;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.http.MediaType;
import org.springframework.security.test.context.support.WithMockUser;
import org.springframework.test.web.servlet.MockMvc;

import java.time.LocalDate;

import static org.hamcrest.Matchers.is;
import static org.springframework.security.test.web.servlet.request.SecurityMockMvcRequestPostProcessors.csrf;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.get;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.post;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.jsonPath;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.status;

@SpringBootTest
@AutoConfigureMockMvc
class AthleteControllerIT {

    @Autowired
    private MockMvc mockMvc;

    @Autowired
    private ObjectMapper objectMapper;

    @Test
    void list_isPubliclyAccessible() throws Exception {
        mockMvc.perform(get("/api/v1/athletes"))
                .andExpect(status().isOk());
    }

    @Test
    @WithMockUser(authorities = "ROLE_ADMIN")
    void create_returnsCreated_forAdmin() throws Exception {
        CreateAthleteRequest req = new CreateAthleteRequest(
                "Eva", "Petrova", "Bulgaria", Gender.FEMALE, LocalDate.of(2002, 5, 1));

        mockMvc.perform(post("/api/v1/athletes")
                        .with(csrf())
                        .contentType(MediaType.APPLICATION_JSON)
                        .content(objectMapper.writeValueAsString(req)))
                .andExpect(status().isCreated())
                .andExpect(jsonPath("$.firstName", is("Eva")))
                .andExpect(jsonPath("$.country", is("Bulgaria")));
    }

    @Test
    @WithMockUser(authorities = "ROLE_ATHLETE")
    void create_returnsForbidden_forAthlete() throws Exception {
        CreateAthleteRequest req = new CreateAthleteRequest(
                "Eva", "Petrova", "Bulgaria", Gender.FEMALE, LocalDate.of(2002, 5, 1));

        mockMvc.perform(post("/api/v1/athletes")
                        .with(csrf())
                        .contentType(MediaType.APPLICATION_JSON)
                        .content(objectMapper.writeValueAsString(req)))
                .andExpect(status().isForbidden());
    }
}
