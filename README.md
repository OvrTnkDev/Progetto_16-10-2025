# **Esercizio-di-gruppo-16-10**

## Gruppo
- Fabio
- Michele
- Marcello

# **Creazione di un SocialNetwork**
## Il programma simula un social network semplificato in cui gli utenti possono:
- creare un profilo personale (base, business o influencer),
- pubblicare post e arricchirli con elementi extra (immagini, tag, hashtag, ecc.),
- seguire altri utenti e ricevere notifiche quando questi pubblicano,
- visualizzare un feed personalizzato in base a diverse modalità (per data, popolarità, hashtag).
- Il sistema è interamente gestito da un’unica istanza centrale, che controlla utenti, post e notifiche.

## **Struttura**
- **Decorator** ---> Tipo Post
- **singleton** -->  SocialNetwork
- **Factory** -----> Utente
- **Observer** ----> Notifiche Post
- **Strategy** ----> Gestione feed