package main

// A simple web server that returns hello world via html/template

import (
	"encoding/json"
	"fmt"
	"html/template"
	"log"
	"net/http"
	"os"
	"time"
)

type message struct {
	Msg string `json:"msg"`
}

func main() {
	indexHtml := `<!DOCTYPE html><html lang="en"><head><meta charset="utf-8"><title>Hello</title></head><body><h1>Hello World!</h1><p>Current time is {{.}}</p></body></html>`
	indexTmpl := template.Must(template.New("index").Parse(indexHtml))

	mux := http.NewServeMux()

	mux.HandleFunc("GET /{$}", func(w http.ResponseWriter, r *http.Request) {
		indexTmpl.Execute(w, time.Now().Format(time.RFC1123))
	})

	mux.HandleFunc("GET /text", func(w http.ResponseWriter, r *http.Request) {
		w.Header().Set("Content-Type", "text/plain")
		w.Write([]byte("Current time is " + time.Now().Format(time.RFC1123)))
	})

	mux.HandleFunc("GET /json", func(w http.ResponseWriter, r *http.Request) {
		m := message{Msg: fmt.Sprintf("Current time is %s", time.Now().Format(time.RFC1123))}
		w.Header().Set("Content-Type", "application/json")
		json.NewEncoder(w).Encode(m)
	})

	port := os.Getenv("PORT")
	if port == "" {
		port = "5000"
	}

	log.Fatal(http.ListenAndServe(":"+port, mux))
}
