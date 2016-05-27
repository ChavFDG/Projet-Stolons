
window.WeekBasket = {};

WeekBasket = window.WeekBasket;

ProductTypesModel = Backbone.Collection.extend({
    url: "/api/ProductTypes",
    
    initialize: function() {
	this.fetch();
    }
});

ProductModel = Backbone.Model.extend({

    //Products id key is 'Id'
    idAttribute: "Id",

    defaults: {},

    /*
      return the html string according to this product's sell type
     */
    getSellTypeString: function() {
	if (this.get("Type") == 0) {
	    return "Au poids";
	} else if (this.get("Type") == 1) {
	    return "À la pièce";
	} else if (this.get("Type") == 2) {
	    return  "Emballé";
	} else {
	    return "Error";
	}
    },

    getStockUnitString: function() {
	if (this.get("Type") == 1) {
	    return "Pièces";
	}
	var productUnit = this.get("ProductUnit");
	var unitsEnum = ["Kg", "Gr", "ml", "L"];
	return unitsEnum[productUnit];
    },

    getPriceUnitString: function() {
	if (this.get("Type") == 1) {
	    return " / Pièce";
	}
	var productUnit = this.get("ProductUnit");
	var unitsEnum = ["Kg", "Gr", "ml", "L"];
	return " / " + unitsEnum[productUnit];
    }
});

ProductsModel = Backbone.Collection.extend(
    {
	defaults: [],

	model: ProductModel,

	url: "/api/Products",

	initialize: function() {
	    this.fetch();
	}
    }
);

TmpWeekBasketModel = Backbone.Model.extend(
    {
	url: "/api/TmpWeekBasket",

	idAttribute: "Id",

	isEmpty: function() {
	    return _.isEmpty(this.get("Products"));
	},

	canPurchase: function() {
	    //TODO move this elsewhere and handle the unconnected case
	    //If there is no id, it means the user is not authenticated, so he can't purchase
	    return ! _.isEmpty(this.get("Id"));
	},

	getTotal: function() {
	    var total = 0;
	    _.forEach(this.get("Products"), function(billEntry) {
		var productModel = WeekBasket.ProductsModel.get(billEntry.ProductId);
		total += (billEntry.Quantity * productModel.get("Price"));
	    });
	    return total;
	},

	getProductEntry: function(productId) {
	    var productEntry;
	    _.forEach(this.get("Products"), function(billEntry) {
		if (billEntry.ProductId == productId) {
		    productEntry = billEntry;
		    return false;
		}
	    });
	    return productEntry;
	},

	addProductToBasket: function(productId) {
	    var self = this;
	    return $.ajax({
                url: '/api/addtobasket',
                data: {
                    productId: productId,
                    weekBasketId: self.get("Id")
                },
                type: 'post',
                success: function (response) {
		    if (response) {
			self.set(JSON.parse(response));
		    }
		}
	    });
	},

	incrementProduct: function(productId) {
	    var self = this;
	    return $.ajax({
                url: '/api/incrementProduct',
                data: {
                    productId: productId,
                    weekBasketId: self.get("Id")
                },
                type: 'post',
                success: function (response) {
		    if (response) {
			self.set(JSON.parse(response));
		    }
		}
	    });
	},

	decrementProduct: function(productId) {
	    var self = this;
	    return $.ajax({
                url: '/api/decrementProduct',
                data: {
                    productId: productId,
                    weekBasketId: self.get("Id")
                },
                type: 'post',
                success: function (response) {
		    if (response) {
			self.set(JSON.parse(response));
		    }
		}
	    });
	},

	removeBillEntry: function(productId) {
	    var self = this;
	    return $.ajax({
                url: '/api/removeBillEntry',
                data: {
                    productId: productId,
                    weekBasketId: self.get("Id")
                },
                type: 'post',
                success: function (response) {
		    if (response) {
			self.set(JSON.parse(response));
		    }
		}
	    });
	},

	resetBasket: function() {
	    var self = this;
	    return $.ajax({
                url: '/api/resetBasket',
                data: {
                    weekBasketId: self.get("Id")
                },
                type: 'post',
                success: function (response) {
		    console.log("Reset basket response: ", response);
		    self.set(JSON.parse(response));
		}
	    });
	}
    });

ValidatedWeekBasketModel = Backbone.Model.extend({

    url: "/api/validatedWeekBasket",

    initialize: function() {
	this.fetch();
    },

    getTotal: function() {
	var total = 0;
	_.forEach(this.get("Products"), function(billEntry) {
		var productModel = WeekBasket.ProductsModel.get(billEntry.ProductId);
	    total += (billEntry.Quantity * productModel.get("Price"));
	});
	return total;
    },
    
    //return true if the basket was validated this week
    exists: function() {
	return ! _.isEmpty(this.get("Products"));
    },

    isEmpty: function() {
	return _.isEmpty(this.get("Products"));
    }
});

var initModels = function() {
    WeekBasket.ProductsModel = new ProductsModel();
    WeekBasket.ProductTypesModel = new ProductTypesModel();
    WeekBasket.ValidatedWeekBasketModel = new ValidatedWeekBasketModel();
    WeekBasket.TmpWeekBasketModel = new TmpWeekBasketModel();
    WeekBasket.ProductsModel.on("sync", function() {
	WeekBasket.TmpWeekBasketModel.fetch();
    });
};

/* ---------------------------  Bellow Views definitions -------------------- */

FiltersView = Backbone.View.extend({

    el: "#filters",

    template: _.template($("#filtersTemplate").html()),

    initialize: function(args) {
	this.model = args.model;
	this.productsModel = args.productsModel;
	this.listenTo(this.model, 'sync change', this.render);
	this.selectedFamily = "Tous";
    },

    onOptionSelected: function(selectedData) {
	this.selectedFamily = selectedData.params.data.id || "Tous";
	this.filterProducts();
    },

    filterProducts: function() {
	var searchTerm = this.$("#search").val();
	this.productsModel.forEach(function(productModel) {
	    var product = productModel.toJSON();
	    if ((this.selectedFamily == "Tous" || (product.Familly && product.Familly.FamillyName == this.selectedFamily)) &&
		(_.isEmpty(searchTerm) || product.Name.toLowerCase().contains(searchTerm) || (product.Description && product.Description.toLowerCase().contains(searchTerm)))) {
		$("#product-" + product.Id).removeClass("hidden");
	    } else {
		$("#product-" + product.Id).removeClass("hidden").addClass("hidden");
	    }
	}, this);
    },

    selectElemTemplate: function(elem) {
	if (!elem.id) {
	    return elem.text;
	}
	var dataImage = $(elem.element).data("image");
	if (!dataImage) {
	    return elem.text;
	} else {
	    return $('<span class="select-option"><img src="' + dataImage +'" />' + $(elem.element).text() + '</span>');
	}
    },

    render: function() {
	this.$el.html(this.template({ productTypes: this.model.toJSON() }));
	this.$('#familiesDropDown').select2({
	    minimumResultsForSearch: Infinity,
	    templateResult: this.selectElemTemplate,
	    templateSelection: this.selectElemTemplate
	});
	this.$('#familiesDropDown').on("select2:select", _.bind(this.onOptionSelected, this));
	this.$('#search').on("input", _.bind(function() {
	    this.filterProducts();
	}, this));
    }
});

ProductModalView = Backbone.View.extend({

    el: "#productModal",

    template: _.template($("#productModalTemplate").html()),

    initialize: function () {
    },

    open: function(productId) {
	this.currentProduct = WeekBasket.ProductsModel.get(productId);
	WeekBasket.TmpWeekBasketModel.on("sync change", this.render, this);
	this.renderModal();
    },

    onClose: function() {
	this.currentProduct = null;
	this.$el.off('hide.bs.modal');
	this.$el.empty();
	WeekBasket.TmpWeekBasketModel.off("sync change", this.render, this);
    },

    render: function() {
	this.$el.html(this.template({product: this.currentProduct.toJSON(), productModel: this.currentProduct}));
    },

    renderModal: function() {
	this.render();
	this.$el.modal({keyboard: true, show: true});
	this.$el.on('hide.bs.modal', _.bind(this.onClose, this));
    }
});


ProducerModalView = Backbone.View.extend({

    el: "#producerModal",

    template: _.template($("#producerModalTemplate").html()),

    open: function(productId) {
	this.renderModal(WeekBasket.ProductsModel.get(productId).get("Producer"));
    },

    onClose: function() {
	this.currentProduct = null;
	this.$el.off('hide.bs.modal');
	this.$el.empty();
    },

    renderModal: function(producer) {
	this.$el.html(this.template({producer: producer}));
	this.$el.modal({keyboard: true, show: true});
	this.$el.on('hide.bs.modal', _.bind(this.onClose, this));
    }
});

ProductView = Backbone.View.extend(
    {
	template: _.template($("#productTemplate").html()),

	initialize: function(args) {
	    this.productId = args.productId;
	    this.el = args.el;
	    this.model = args.model;
	    this.model.on("sync change", this.render, this);
	},

	render: function() {
	    this.$el.html(this.template({product: this.model.toJSON(), productModel: this.model}));
	}
    }
);

ProductActionView = Backbone.View.extend(
    {
	template: _.template($("#productActionTemplate").html()),

	events: {
	    "click .minus": "decrement",
	    "click .plus": "increment",
	    "click .addProductBtn": "addToBasket"
	},

	initialize: function(args) {
	    this.el = args.el;
	    this.productId = args.productId;
	    this.model = args.model;
	    this.model.on("sync change", this.basketChanged, this);
	},

	basketChanged: function() {
	    this.billEntry = this.model.getProductEntry(this.productId);
	    this.render();
	},

	canIncrement: function() {
	    if (!this.billEntry) {
		return false;
	    }
	    return this.billEntry.Quantity < this.billEntry.Product.RemainingStock;
	},

	addToBasket: function() {
	    this.$(".productQuantityChanger").addClass("hidden");
	    this.$(".productQuantityLoading").removeClass("hidden");
	    WeekBasket.TmpWeekBasketModel.addProductToBasket(this.productId).then(_.bind(function() {
		this.$(".productQuantityLoading").addClass("hidden");
		this.$(".productQuantityChanger").removeClass("hidden");
	    }, this));
	},

	increment: function() {
	    this.$(".productQuantityChanger").addClass("hidden");
	    this.$(".productQuantityLoading").removeClass("hidden");
	    WeekBasket.TmpWeekBasketModel.incrementProduct(this.productId).then(_.bind(function() {
		this.$(".productQuantityLoading").addClass("hidden");
		this.$(".productQuantityChanger").removeClass("hidden");
	    }, this));
	},

	decrement: function() {
	    this.$(".productQuantityChanger").addClass("hidden");
	    this.$(".productQuantityLoading").removeClass("hidden");
	    WeekBasket.TmpWeekBasketModel.decrementProduct(this.productId).then(_.bind(function() {
		this.$(".productQuantityLoading").addClass("hidden");
		this.$(".productQuantityChanger").removeClass("hidden");
	    }, this));
	},

	render: function() {
	    this.$el.html(this.template({billEntry: this.billEntry, canIncrement: this.canIncrement}));
	}
    }
);

ProductsView = Backbone.View.extend(
    {
	el: "#products",

	template: _.template($("#productsTemplate").html()),

	initialize: function(args) {
	    this.model = args.model;
	    this.model.on("sync", this.render, this);
	    this.tmpBasketModel = args.tmpBasketModel;
	    this.views = {};
	},

	render: function() {
	    this.$el.html(this.template({products: this.model.models}));
	    this.model.forEach(function(productModel) {
		var productView = new ProductView({
		    el: "#product-" + productModel.get("Id"),
		    model: productModel
		});
		this.views[productModel.get("Id")] = productView;
		productView.render();

		var productActionView = new ProductActionView(
		    {
			model: this.tmpBasketModel,
			productId: productModel.get("Id"),
			el: "#product-" + productModel.get("Id") + " .pr_actions"
		    });
		productActionView.render();
	    }, this);
	}
    }
);

TmpWeekBasketView = Backbone.View.extend(
    {
	el: "#tmpBasket",

	template: _.template($("#tmpWeekBasketTemplate").html()),

	initialize: function(args) {
	    this.model = args.model;
	    this.model.on("sync change", this.render, this);
	    this.validatedBasketModel = args.validatedBasketModel;
	},

	render: function () {
	    this.$el.html(this.template({tmpBasketModel: this.model, tmpBasket: this.model.toJSON(), validatedBasketModel: this.validatedBasketModel}));
	}
    }
);

ValidatedWeekBasketView = Backbone.View.extend(
    {
	el: "#validatedBasket",

	template: _.template($("#validatedWeekBasketTemplate").html()),

	events: {
	    "click .validatedBasketCollapse": "toggleCollapse"
	},

	initialize: function(args) {
	    this.model = args.model;
	    this.model.on("sync", this.render, this);
	    this.tmpBasketModel = args.tmpBasketModel;
	    this.tmpBasketModel.on("sync change", this.renderCollapse, this);
	},

	toggleCollapse: function() {
	    if (this.$("#collapsible").hasClass("in")) {
		this.collapse(true);
	    } else {
		this.collapse(false);
	    }
	},

	collapse: function(hide) {
	    if (hide) {
		this.$("#collapsible").collapse("hide");
		this.$(".glyphicon-collapse-up").addClass("hidden");
		this.$(".glyphicon-collapse-down").removeClass("hidden");
	    } else {
		this.$("#collapsible").collapse("show");
		this.$(".glyphicon-collapse-up").removeClass("hidden");
		this.$(".glyphicon-collapse-down").addClass("hidden");
	    }
	},

	renderCollapse: function() {
	    if (this.tmpBasketModel.isEmpty() || this.tmpBasketModel.get("Validated") === true) {
		this.collapse(false);
	    } else if (this.$("#collapsible").hasClass("in")) {
		//Hide only if it is visible
		this.collapse(true);
	    }
	},

	render: function () {
	    this.$el.html(this.template({validatedBasketModel: this.model, validatedBasket: this.model.toJSON()}));
	    this.renderCollapse();
	}
    }
);

var initViews = function() {

    WeekBasket.ProductModalView = new ProductModalView();

    WeekBasket.ProducerModalView = new ProducerModalView();

    WeekBasket.FiltersView = new FiltersView({ model: WeekBasket.ProductTypesModel, productsModel: WeekBasket.ProductsModel });

    WeekBasket.ValidatedWeekBasketView = new ValidatedWeekBasketView(
	{
	    model: WeekBasket.ValidatedWeekBasketModel,
	    tmpBasketModel: WeekBasket.TmpWeekBasketModel
	});

    WeekBasket.ProductsView = new ProductsView({ model: WeekBasket.ProductsModel, tmpBasketModel: WeekBasket.TmpWeekBasketModel });

    WeekBasket.TmpWeekBasketView = new TmpWeekBasketView({
	model: WeekBasket.TmpWeekBasketModel,
	validatedBasketModel: WeekBasket.ValidatedWeekBasketModel,
	validatedBasketView: WeekBasket.ValidatedWeekBasketView
    });
};

var bootstrapWeekBasket = function() {
    initModels();
    initViews();
};

$(function() {
    bootstrapWeekBasket();
});