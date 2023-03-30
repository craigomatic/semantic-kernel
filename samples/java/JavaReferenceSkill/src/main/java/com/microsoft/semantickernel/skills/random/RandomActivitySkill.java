// Copyright (c) Microsoft. All rights reserved.

package com.microsoft.semantickernel.skills.random;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.microsoft.azure.functions.*;
import com.microsoft.azure.functions.annotation.FunctionName;
import com.microsoft.azure.functions.annotation.HttpTrigger;

import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.util.concurrent.CompletableFuture;

/**
 * This class defines an Azure Function that fetches a random activity from an API.
 */
public class RandomActivitySkill {

    /**
     * Fetches a random activity from the API and returns it in the HTTP response.
     *
     * @param request The incoming HTTP request containing the route and request parameters.
     * @param context The execution context, which contains the logger and other metadata.
     * @return A CompletableFuture of HttpResponseMessage containing the random activity.
     */
    @FunctionName("GetRandomActivity")
    public CompletableFuture<HttpResponseMessage> getRandomActivityAsync(
            @HttpTrigger(name = "request", methods = HttpMethod.GET, route = "getRandomActivity") HttpRequestMessage<String> request,
            ExecutionContext context) {

        HttpClient httpClient = HttpClient.newHttpClient();
        HttpRequest httpRequest = HttpRequest.newBuilder()
                .uri(URI.create("https://www.boredapi.com/api/activity"))
                .build();

        return httpClient.sendAsync(httpRequest, HttpResponse.BodyHandlers.ofString())
                .thenApply(response -> {
                    ObjectMapper objectMapper = new ObjectMapper();
                    try {
                        Activity activity = objectMapper.readValue(response.body(), Activity.class);
                        return request.createResponseBuilder(HttpStatus.OK).body(activity.activity()).build();
                    } catch (Exception e) {
                        context.getLogger().severe("Error deserializing JSON: " + e.getMessage());
                        return request.createResponseBuilder(HttpStatus.INTERNAL_SERVER_ERROR).body("Error deserializing JSON").build();
                    }
                });
    }
}
