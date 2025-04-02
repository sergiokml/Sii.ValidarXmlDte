[![](https://img.shields.io/badge/License-GPLv3-blue.svg?style=for-the-badge)](LICENSE.txt)
[![.NET](https://img.shields.io/badge/.NET-8.0-blueviolet?style=for-the-badge)](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
[![GitHub commit activity](https://img.shields.io/github/commit-activity/w/sergiokml/Sii.ValidarXmlDte?style=for-the-badge)](https://github.com/sergiokml/Sii.ValidarXmlDte)
[![GitHub contributors](https://img.shields.io/github/contributors/sergiokml/Sii.ValidarXmlDte?style=for-the-badge)](https://github.com/sergiokml/Sii.ValidarXmlDte/graphs/contributors/)
[![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/sergiokml/Sii.ValidarXmlDte?style=for-the-badge)](https://github.com/sergiokml/Sii.ValidarXmlDte)


# Valida XML DTE

This solution allows you to validate DTE (Electronic Tax Documents) from Chile's [Servicio de Impuestos Internos (SII)](https://www.sii.cl/) against XSD schemas. It is designed as a lightweight API that processes one or more DTEs and returns validation results, highlighting schema errors or warnings.

It includes:

- Minimal API for local or cloud-based validation
- XML input (ISO-8859-1) with one or more DTEs
- Fast response format with IDs, errors, and warnings

> This repository has no relationship with the government entity [SII](https://www.sii.cl/), only for educational purposes.

---

### 📆 Supported DTE Types

- `DTE`
- `EnvioDTE`
- `EnvioBOLETA`
- `RespuestaDTE`
- `EnvioRecibos`
- `RESULTADO_ENVIO`

---

### 🚀 Usage

Once the app is running, you can sign DTEs via::

```bash
curl -X POST http://localhost:5167/api/dte/validar \
  -H "Content-Type: application/xml" \
  -d '{
    "dtes": [
      "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?><DTE>...</DTE>",
      "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?><DTE>...</DTE>"
    ]
  }'
```

The response will be:

```json
[
  {
    "id": "D33F2178",
    "errors": [],
    "warnings": []
  },
  {
    "id": "D33F2179",
    "errors": ["Missing mandatory element: Receptor"],
    "warnings": []
  }
]
```

---

### 📢 Have a question? Found a Bug?

Feel free to **file a new issue** with a respective title and description on the [Sii.ValidarXmlDte/issues](https://github.com/sergiokml/ValidarXmlDte/issues) repository.

---

### 💖 Community and Contributions

If this tool was useful, consider contributing with ideas or improving it further.


<p align="center">
    <a href="https://www.paypal.com/donate/?hosted_button_id=PTKX9BNY96SNJ" target="_blank">
        <img width="12%" src="https://img.shields.io/badge/PayPal-00457C?style=for-the-badge&logo=paypal&logoColor=white" alt="Azure Function">
    </a>
</p>

---

### 📘 License

This repository is released under the [GNU General Public License v3.0](LICENSE.txt).