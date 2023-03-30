package com.microsoft.semantickernel.skills.random;

import com.microsoft.azure.functions.ExecutionContext;
import com.microsoft.azure.functions.HttpRequestMessage;
import com.microsoft.azure.functions.HttpResponseMessage;
import com.microsoft.azure.functions.HttpStatus;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;

import java.io.IOException;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.util.logging.Logger;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.mockito.Mockito.*;

class RandomActivitySkillTest {

    @Test
    @DisplayName("should return a random activity when the API call is successful")
    void getRandomActivityWhenApiCallIsSuccessful() throws IOException, InterruptedException {
        var request = mock(HttpRequestMessage.class);
        ExecutionContext context = mock(ExecutionContext.class);
        Logger logger = mock(Logger.class);
        HttpClient httpClient = mock(HttpClient.class);
        HttpRequest httpRequest = mock(HttpRequest.class);
        var response = mock(HttpResponse.class);

        when(context.getLogger()).thenReturn(logger);
        when(HttpClient.newHttpClient()).thenReturn(httpClient);
        when(httpClient.send(httpRequest, HttpResponse.BodyHandlers.ofString()))
                .thenReturn(response);
        when(response.body())
                .thenReturn(
                        "{\"activity\":\"Test Activity\",\"type\":\"Test Type\",\"participants\":1,\"price\":0.0,\"link\":\"Test Link\",\"key\":\"Test Key\",\"accessibility\":0.0}");

        HttpResponseMessage actual = new RandomActivitySkill().getRandomActivity(request, context);

        assertEquals(HttpStatus.OK, actual.getStatus());
        assertEquals("Test Activity", actual.getBody());
    }
}