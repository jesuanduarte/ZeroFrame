document.addEventListener("DOMContentLoaded", async () => {
    if (!ZeroFrameApi.protegerPagina()) return;

    const lista = document.querySelector(".addresses-list");
    const addAddressBtn = document.querySelector("#addAddressBtn");
    if (!lista) return;

    addAddressBtn?.addEventListener("click", () => {
        if (lista.querySelector(".address-card-new")) return;
        lista.prepend(criarCardEditor());
    });

    lista.addEventListener("click", async (event) => {
        const removerBtn = event.target.closest("[data-remover-endereco]");
        if (removerBtn) {
            try {
                removerBtn.disabled = true;
                await EnderecoService.remover(removerBtn.dataset.removerEndereco);
                await carregarEnderecos();
            } catch (error) {
                ZeroFrameApi.mostrarMensagem(lista, ZeroFrameApi.tratarErro(error, "Não foi possível remover o endereço."));
            } finally {
                removerBtn.disabled = false;
            }
            return;
        }

        const editarBtn = event.target.closest("[data-editar-endereco]");
        if (editarBtn) {
            const card = editarBtn.closest(".address-card");
            const form = card.querySelector(".address-edit-form");
            const view = card.querySelector(".address-view");
            preencherFormulario(form, lerEnderecoDoVisualizacao(card));
            view.classList.add("hidden");
            form.classList.remove("hidden");
            return;
        }

        const cancelarBtn = event.target.closest("[data-cancelar-edicao]");
        if (cancelarBtn) {
            const card = cancelarBtn.closest(".address-card");
            const form = card.querySelector(".address-edit-form");
            const view = card.querySelector(".address-view");
            if (card.classList.contains("address-card-new")) {
                card.remove();
            } else {
                form.classList.add("hidden");
                view.classList.remove("hidden");
            }
        }
    });

    async function carregarEnderecos() {
        try {
            ZeroFrameApi.mostrarCarregando(lista, "Carregando endereços...");
            const enderecos = await EnderecoService.listar();
            lista.textContent = "";

            if (!enderecos.length) {
                ZeroFrameApi.mostrarMensagem(lista, "Nenhum endereço cadastrado.");
                return;
            }

            enderecos.forEach((endereco, index) => {
                lista.appendChild(criarCardEndereco(endereco, index));
            });
        } catch (error) {
            ZeroFrameApi.mostrarMensagem(lista, ZeroFrameApi.tratarErro(error, "Erro ao carregar endereços."));
        }
    }

    function criarCardEndereco(endereco, index) {
        const card = document.createElement("div");
        card.className = "address-card";
        card.dataset.id = endereco.id || endereco.Id || "";

        const header = document.createElement("div");
        header.className = "address-card-header";

        const labelWrapper = document.createElement("div");
        labelWrapper.className = "h-e-label";

        const title = document.createElement("h3");
        title.textContent = index === 0 ? "Casa" : "Endereço";

        const label = document.createElement("span");
        label.className = "address-label";
        label.textContent = index === 0 ? "Principal" : "Secundário";

        const buttonsWrapper = document.createElement("div");
        buttonsWrapper.className = "address-actions";

        const editButton = document.createElement("button");
        editButton.className = "edit-address";
        editButton.type = "button";
        editButton.dataset.editarEndereco = endereco.id || endereco.Id || "";
        editButton.setAttribute("aria-label", "Editar endereço");
        const editIcon = document.createElement("i");
        editIcon.className = "fa-solid fa-pen-to-square";
        editButton.appendChild(editIcon);

        const removeButton = document.createElement("button");
        removeButton.className = "edit-address";
        removeButton.type = "button";
        removeButton.dataset.removerEndereco = endereco.id || endereco.Id || "";
        removeButton.setAttribute("aria-label", "Remover endereço");
        const removeIcon = document.createElement("i");
        removeIcon.className = "fa-solid fa-trash";
        removeButton.appendChild(removeIcon);

        const view = document.createElement("div");
        view.className = "address-view";

        const rua = document.createElement("p");
        rua.className = "rua";
        rua.textContent = `${endereco.rua || endereco.Rua || ""}, ${endereco.numero || endereco.Numero || ""}`;

        const bairro = document.createElement("p");
        bairro.className = "bairro";
        bairro.textContent = endereco.bairro || endereco.Bairro || "";

        const complemento = document.createElement("p");
        complemento.className = "complemento";
        const textoComplemento = endereco.complemento || endereco.Complemento || "";
        complemento.textContent = textoComplemento ? `Complemento: ${textoComplemento}` : "";
        if (!textoComplemento) {
            complemento.style.display = "none";
        }

        const cidade = document.createElement("p");
        cidade.className = "cidade";
        cidade.textContent = `${endereco.cidade || endereco.Cidade || ""} - ${endereco.estado || endereco.Estado || ""}`;

        const cep = document.createElement("p");
        cep.className = "cep";
        cep.textContent = `CEP ${endereco.cep || endereco.Cep || endereco.CEP || ""}`;

        const telefone = document.createElement("p");
        telefone.className = "telefone";
        telefone.textContent = endereco.telefone || endereco.Telefone || "";

        const form = criarEnderecoForm(endereco, {
            isNew: false,
            onSave: async (dados) => {
                await salvarEdicao(endereco.id || endereco.Id || "", dados);
            },
            onCancel: () => {
                form.classList.add("hidden");
                view.classList.remove("hidden");
            }
        });

        labelWrapper.append(title, label);
        buttonsWrapper.append(editButton, removeButton);
        header.append(labelWrapper, buttonsWrapper);
        view.append(rua, bairro, complemento, cidade, cep, telefone);
        card.append(header, view, form);

        return card;
    }

    function criarCardEditor() {
        const card = document.createElement("div");
        card.className = "address-card address-card-new";

        const header = document.createElement("div");
        header.className = "address-card-header";

        const labelWrapper = document.createElement("div");
        labelWrapper.className = "h-e-label";

        const title = document.createElement("h3");
        title.textContent = "Novo endereço";

        const label = document.createElement("span");
        label.className = "address-label";
        label.textContent = "Novo";

        labelWrapper.append(title, label);
        header.append(labelWrapper);

        const form = criarEnderecoForm({}, {
            isNew: true,
            onSave: async (dados) => {
                await salvarNovoEndereco(dados);
            },
            onCancel: () => card.remove()
        });

        card.append(header, form);
        return card;
    }

    function criarEnderecoForm(endereco, { isNew, onSave, onCancel }) {
        const form = document.createElement("form");
        form.className = "address-edit-form";
        if (!isNew) form.classList.add("hidden");

        const campos = [
            { name: "rua", label: "Rua", required: true },
            { name: "numero", label: "Número", required: true },
            { name: "bairro", label: "Bairro", required: true },
            { name: "complemento", label: "Complemento", required: false },
            { name: "cidade", label: "Cidade", required: true },
            { name: "estado", label: "Estado", required: true },
            { name: "cep", label: "CEP", required: true }
        ];

        campos.forEach((campo) => {
            const fieldLabel = document.createElement("label");
            fieldLabel.textContent = campo.label + (campo.required ? "" : " (opcional)");

            const input = document.createElement("input");
            input.name = campo.name;
            input.type = campo.name === "cep" ? "text" : campo.name === "telefone" ? "tel" : "text";
            input.placeholder = campo.label;
            input.value = endereco[campo.name] || endereco[campo.name.charAt(0).toUpperCase() + campo.name.slice(1)] || "";
            if (campo.required) {
                input.required = true;
            }

            fieldLabel.appendChild(input);
            form.appendChild(fieldLabel);
        });

        const buttons = document.createElement("div");
        buttons.className = "address-buttons";

        const saveButton = document.createElement("button");
        saveButton.type = "submit";
        saveButton.className = "save-address";
        saveButton.textContent = isNew ? "Adicionar" : "Salvar";

        const cancelButton = document.createElement("button");
        cancelButton.type = "button";
        cancelButton.className = "cancel-address";
        cancelButton.dataset.cancelarEdicao = "true";
        cancelButton.textContent = "Cancelar";

        buttons.append(saveButton, cancelButton);
        form.appendChild(buttons);

        form.addEventListener("submit", async (event) => {
            event.preventDefault();
            const dados = lerEnderecoDoForm(form);
            try {
                await onSave(dados);
            } catch (error) {
                ZeroFrameApi.notificar(ZeroFrameApi.tratarErro(error, "Não foi possível salvar o endereço."));
            }
        });

        cancelButton.addEventListener("click", (event) => {
            event.preventDefault();
            onCancel();
        });

        return form;
    }

    function lerEnderecoDoForm(form) {
        const formData = new FormData(form);
        return {
            rua: formData.get("rua")?.toString().trim() || "",
            numero: formData.get("numero")?.toString().trim() || "",
            bairro: formData.get("bairro")?.toString().trim() || "",
            complemento: formData.get("complemento")?.toString().trim() || "",
            cidade: formData.get("cidade")?.toString().trim() || "",
            estado: normalizarEstado(formData.get("estado")?.toString().trim() || ""),
            cep: formData.get("cep")?.toString().trim() || ""
        };
    }

    function normalizarEstado(valor) {
        const estados = {
            "acre": "AC", "alagoas": "AL", "amapa": "AP", "amapá": "AP", "amazonas": "AM",
            "bahia": "BA", "ceara": "CE", "ceará": "CE", "distrito federal": "DF",
            "espirito santo": "ES", "espírito santo": "ES", "goias": "GO", "goiás": "GO",
            "maranhao": "MA", "maranhão": "MA", "mato grosso": "MT", "mato grosso do sul": "MS",
            "minas gerais": "MG", "para": "PA", "pará": "PA", "paraiba": "PB", "paraíba": "PB",
            "parana": "PR", "paraná": "PR", "pernambuco": "PE", "piaui": "PI", "piauí": "PI",
            "rio de janeiro": "RJ", "rio grande do norte": "RN", "rio grande do sul": "RS",
            "rondonia": "RO", "rondônia": "RO", "roraima": "RR", "santa catarina": "SC",
            "sao paulo": "SP", "são paulo": "SP", "sergipe": "SE", "tocantins": "TO"
        };
        const normalizado = valor.toLowerCase();
        return (estados[normalizado] || valor).toUpperCase().slice(0, 2);
    }

    function lerEnderecoDoVisualizacao(card) {
        return {
            rua: card.querySelector(".rua")?.textContent?.replace(/,\s*\d+$/, "")?.trim() || "",
            numero: (card.querySelector(".rua")?.textContent?.match(/,\s*(\d+)$/) || [])[1] || "",
            bairro: card.querySelector(".bairro")?.textContent?.trim() || "",
            complemento: card.querySelector(".complemento")?.textContent?.trim() || "",
            cidade: card.querySelector(".cidade")?.textContent?.split(" - ")[0]?.trim() || "",
            estado: card.querySelector(".cidade")?.textContent?.split(" - ")[1]?.trim() || "",
            cep: card.querySelector(".cep")?.textContent?.replace(/CEP\s*/i, "").trim() || "",
            telefone: card.querySelector(".telefone")?.textContent?.trim() || ""
        };
    }

    function preencherFormulario(form, endereco) {
        Object.entries(endereco).forEach(([key, value]) => {
            const input = form.querySelector(`[name="${key}"]`);
            if (input) input.value = value || "";
        });
    }

    async function salvarEdicao(enderecoId, dados) {
        await EnderecoService.editar(enderecoId, dados);
        ZeroFrameApi.notificar("Endereço atualizado com sucesso.", "sucesso");
        await carregarEnderecos();
    }

    async function salvarNovoEndereco(dados) {
        await EnderecoService.adicionar(dados);
        ZeroFrameApi.notificar("Endereço adicionado com sucesso.", "sucesso");
        await carregarEnderecos();
    }

    await carregarEnderecos();
});
