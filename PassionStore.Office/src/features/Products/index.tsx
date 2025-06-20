import { useState, useEffect, useCallback } from "react";
import {
  Box,
  TextField,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Pagination,
  Typography,
  Paper,
  Chip,
  Rating,
  IconButton,
  InputAdornment,
  Tooltip,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Switch,
  FormControlLabel,
  Grid,
  CircularProgress,
  Alert,
  Snackbar,
  Select,
  MenuItem,
  InputLabel,
  FormControl,
  Popover,
  Button,
  Checkbox,
} from "@mui/material";
import {
  Search as SearchIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  AddCircle as AddCircleIcon,
  Refresh as RefreshIcon,
  Inventory as InventoryIcon,
  Close as CloseIcon,
  Save as SaveIcon,
  Upload as UploadIcon,
  ContentCopy as ContentCopyIcon,
  ExpandMore as ExpandMoreIcon,
  Clear as ClearIcon,
  RemoveRedEye as EyeIcon,
} from "@mui/icons-material";
import { setParams, setPageNumber, setCreateFormOpen, setSelectedProductId, setDeleteDialogOpen } from "./productsSlice";
import { useAppDispatch, useAppSelector } from "../../app/store/store";
import {
  useFetchProductsQuery,
  useFetchProductByIdQuery,
  useCreateProductMutation,
  useUpdateProductMutation,
  useDeleteProductMutation,
} from "../../app/api/productApi";
import {
  useCreateProductVariantMutation,
  useUpdateProductVariantMutation,
  useDeleteProductVariantMutation,
} from "../../app/api/productVariantApi";
import { useFetchCategoriesTreeQuery } from "../../app/api/categoryApi";
import { useFetchBrandsTreeQuery } from "../../app/api/brandApi";
import { useFetchColorsTreeQuery } from "../../app/api/colorApi";
import { useFetchSizesTreeQuery } from "../../app/api/sizeApi";
import { Category } from "../../app/models/responses/category";
import { debounce } from "lodash";
import { Brand } from "../../app/models/responses/brand";
import { Color } from "../../app/models/responses/color";
import { Size } from "../../app/models/responses/size";
import { PaginationParams } from "../../app/models/params/pagination";

interface Product {
  id: string;
  name: string;
  description: string;
  inStock: boolean;
  isFeatured: boolean;
  brand: { id: string; name: string; description?: string };
  categories: { id: string; name: string; subCategories: Category[] }[];
  productImages: { id: string; imageUrl: string; isMain: boolean }[];
  productVariants: ProductVariant[];
  minPrice: number;
  maxPrice: number;
  averageRating: number;
}

interface ProductVariant {
  id: string;
  price: number;
  stockQuantity: number;
  color: { id: string; name: string };
  size: { id: string; name: string };
  images: { id: string; imageUrl: string; isMain: boolean }[];
}

interface ProductRequest {
  name: string;
  description: string;
  inStock: boolean;
  isFeatured: boolean;
  brandId: string;
  categoryIds: string[];
  formImages: File[];
  existingImages: { id: string; isMain: boolean }[];
}

interface ProductVariantRequest {
  price: number;
  stockQuantity: number;
  colorId: string;
  sizeId: string;
  formImages: File[];
  existingImages: { id: string; isMain: boolean }[];
}

interface CategoryMenuProps {
  categories: Category[];
  depth: number;
  selectedCategoryIds: { id: string; name: string }[];
  onSelect: (categoryId: string, categoryName: string) => void;
}

const CategoryMenu = ({ categories, depth, selectedCategoryIds, onSelect }: CategoryMenuProps) => {
  const [openMenus, setOpenMenus] = useState<{ [key: string]: HTMLElement | null }>({});

  const toggleMenu = (categoryId: string, anchorEl: HTMLElement | null) => {
    setOpenMenus((prev) => ({
      ...prev,
      [categoryId]: anchorEl,
    }));
  };

  const closeMenu = (categoryId: string) => {
    setOpenMenus((prev) => ({
      ...prev,
      [categoryId]: null,
    }));
  };

  return (
    <>
      {categories.map((category) => {
        const hasChildren = category.subCategories && category.subCategories.length > 0;
        const isLeaf = !hasChildren;
        const isSelected = selectedCategoryIds.some((item) => item.id === category.id);

        return (
          <Box key={category.id}>
            <MenuItem
              sx={{
                pl: 2 + depth * 2,
                color: isLeaf ? "text.primary" : "text.secondary",
                fontStyle: isLeaf ? "normal" : "italic",
                backgroundColor: isSelected ? "action.selected" : "inherit",
              }}
              selected={isSelected}
              onClick={(e) => {
                if (isLeaf) {
                  onSelect(category.id, category.name);
                } else {
                  toggleMenu(category.id, e.currentTarget);
                }
              }}
            >
              <Box sx={{ display: "flex", alignItems: "center", width: "100%" }}>
                <Typography>{category.name}</Typography>
                {hasChildren && (
                  <IconButton
                    size="small"
                    onClick={(e) => {
                      e.stopPropagation();
                      toggleMenu(category.id, e.currentTarget);
                    }}
                    sx={{ ml: "auto" }}
                  >
                    <ExpandMoreIcon
                      sx={{
                        transform: openMenus[category.id] ? "rotate(180deg)" : "rotate(0deg)",
                        transition: "transform 0.2s",
                      }}
                    />
                  </IconButton>
                )}
              </Box>
            </MenuItem>
            {hasChildren && (
              <Popover
                open={Boolean(openMenus[category.id])}
                anchorEl={openMenus[category.id]}
                onClose={() => closeMenu(category.id)}
                anchorOrigin={{
                  vertical: "top",
                  horizontal: "right",
                }}
                transformOrigin={{
                  vertical: "top",
                  horizontal: "left",
                }}
                PaperProps={{
                  sx: { minWidth: 200 },
                }}
              >
                <CategoryMenu
                  categories={category.subCategories}
                  depth={depth + 1}
                  selectedCategoryIds={selectedCategoryIds}
                  onSelect={onSelect}
                />
              </Popover>
            )}
          </Box>
        );
      })}
    </>
  );
};

interface VariantDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  product: Product | null;
}

const VariantDialog = ({ open, onOpenChange, product }: VariantDialogProps) => {
  if (!product) return null;

  const getStockStatus = (stock: number) => {
    if (stock === 0) return { label: "Out of Stock", variant: "error" as const };
    if (stock < 10) return { label: "Low Stock", variant: "warning" as const };
    return { label: "In Stock", variant: "success" as const };
  };

  const formatVND = (price: number) => {
    return new Intl.NumberFormat("vi-VN", {
      style: "currency",
      currency: "VND",
      minimumFractionDigits: 0,
    }).format(price);
  };

  return (
    <Dialog open={open} onClose={() => onOpenChange(false)} fullWidth maxWidth="lg">
      <DialogTitle>Product Variants - {product.name}</DialogTitle>
      <DialogContent>
        <TableContainer component={Paper} elevation={0}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Variant Name</TableCell>
                <TableCell>SKU</TableCell>
                <TableCell>Price</TableCell>
                <TableCell>Stock</TableCell>
                <TableCell>Status</TableCell>
                <TableCell>Size</TableCell>
                <TableCell>Color</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {product.productVariants.map((variant: ProductVariant) => {
                const stockStatus = getStockStatus(variant.stockQuantity);
                const variantName = `${variant.size.name} - ${variant.color.name}`;
                return (
                  <TableRow key={variant.id}>
                    <TableCell>{variantName}</TableCell>
                    <TableCell sx={{ fontFamily: "monospace" }}>{variant.id}</TableCell>
                    <TableCell>{formatVND(variant.price)}</TableCell>
                    <TableCell>{variant.stockQuantity}</TableCell>
                    <TableCell>
                      <Chip
                        label={stockStatus.label}
                        size="small"
                        color={stockStatus.variant}
                        variant="outlined"
                      />
                    </TableCell>
                    <TableCell>{variant.size.name}</TableCell>
                    <TableCell>{variant.color.name}</TableCell>
                  </TableRow>
                );
              })}
            </TableBody>
          </Table>
        </TableContainer>
        <Box
          sx={{
            mt: 4,
            display: "grid",
            gridTemplateColumns: "repeat(3, 1fr)",
            gap: 2,
            p: 2,
            bgcolor: "grey.100",
            borderRadius: 1,
          }}
        >
          <Box sx={{ textAlign: "center" }}>
            <Typography variant="h6">{product.productVariants.length}</Typography>
            <Typography variant="body2" color="textSecondary">
              Total Variants
            </Typography>
          </Box>
          <Box sx={{ textAlign: "center" }}>
            <Typography variant="h6">
              {product.productVariants.reduce((sum: number, v: ProductVariant) => sum + v.stockQuantity, 0)}
            </Typography>
            <Typography variant="body2" color="textSecondary">
              Total Stock
            </Typography>
          </Box>
          <Box sx={{ textAlign: "center" }}>
            <Typography variant="h6">
              {formatVND(product.minPrice)} - {formatVND(product.maxPrice)}
            </Typography>
            <Typography variant="body2" color="textSecondary">
              Price Range
            </Typography>
          </Box>
        </Box>
      </DialogContent>
      <DialogActions>
        <Button onClick={() => onOpenChange(false)}>Close</Button>
      </DialogActions>
    </Dialog>
  );
};

const formatVND = (price: number) => {
  return new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: "VND",
    minimumFractionDigits: 0,
  }).format(price);
};

export default function ProductList() {
  const dispatch = useAppDispatch();
  const { params, selectedProductId, isCreateFormOpen, isDeleteDialogOpen } = useAppSelector(
    (state) => state.product
  );
  const { data, isLoading, error, refetch, isFetching } = useFetchProductsQuery(params);
  const [search, setSearch] = useState(params.searchTerm || "");
  const [selectedProductIds, setSelectedProductIds] = useState<string[]>([]);
  const [viewingVariants, setViewingVariants] = useState<Product | null>(null);

  const { data: selectedProduct, isLoading: isLoadingProduct } = useFetchProductByIdQuery(
    selectedProductId || "",
    {
      skip: !selectedProductId || isDeleteDialogOpen,
    }
  );

  const { data: categoriesData, isLoading: isLoadingCategories } = useFetchCategoriesTreeQuery();
  const { data: brandsData, isLoading: isLoadingBrands } = useFetchBrandsTreeQuery();
  const { data: sizesData, isLoading: isLoadingSizes } = useFetchSizesTreeQuery();
  const { data: colorsData, isLoading: isLoadingColors } = useFetchColorsTreeQuery();

  const [createProduct, { isLoading: isCreatingProduct }] = useCreateProductMutation();
  const [updateProduct, { isLoading: isUpdatingProduct }] = useUpdateProductMutation();
  const [deleteProduct, { isLoading: isDeleting }] = useDeleteProductMutation();
  const [createProductVariant, { isLoading: isCreatingVariant }] = useCreateProductVariantMutation();
  const [updateProductVariant, { isLoading: isUpdatingVariant }] = useUpdateProductVariantMutation();
  const [deleteProductVariant, { isLoading: isDeletingVariant }] = useDeleteProductVariantMutation();

  const [formData, setFormData] = useState<ProductRequest>({
    name: "",
    description: "",
    inStock: true,
    isFeatured: false,
    brandId: "",
    categoryIds: [],
    formImages: [],
    existingImages: [],
  });

  const [productVariants, setProductVariants] = useState<ProductVariantRequest[]>([]);
  const [previewUrls, setPreviewUrls] = useState<string[]>([]);
  const [, setDeletedImageIds] = useState<string[]>([]);
  const [notification, setNotification] = useState({
    open: false,
    message: "",
    severity: "success" as "success" | "error" | "info" | "warning",
  });
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const [selectedCategoryIds, setSelectedCategoryIds] = useState<{ id: string; name: string }[]>([]);

  const [errors, setErrors] = useState<{
    name?: string;
    description?: string;
    brandId?: string;
    formImages?: string;
    existingImages?: string;
    variants?: string;
  }>({});

  const [newVariantForm, setNewVariantForm] = useState<ProductVariantRequest>({
    price: 0,
    stockQuantity: 0,
    colorId: "",
    sizeId: "",
    formImages: [],
    existingImages: [],
  });
  const [variantErrors, setVariantErrors] = useState<{
    sizeId?: string;
    colorId?: string;
    price?: string;
    stockQuantity?: string;
    formImages?: string;
  }>({});
  const [variantPreviewUrls, setVariantPreviewUrls] = useState<string[]>([]);
  const [deletedVariantIds, setDeletedVariantIds] = useState<string[]>([]);

  useEffect(() => {
    const urls = formData.formImages.map((file) => URL.createObjectURL(file));
    setPreviewUrls(urls);
    return () => {
      urls.forEach((url) => URL.revokeObjectURL(url));
    };
  }, [formData.formImages]);

  useEffect(() => {
    const urls = newVariantForm.formImages.map((file) => URL.createObjectURL(file));
    setVariantPreviewUrls(urls);
    return () => {
      urls.forEach((url) => URL.revokeObjectURL(url));
    };
  }, [newVariantForm.formImages]);

  const debouncedSearch = useCallback(
    debounce((value: string) => {
      dispatch(setParams({ searchTerm: value.trim() || undefined }));
      dispatch(setPageNumber(1));
    }, 500),
    [dispatch]
  );

  useEffect(() => {
    if (isCreateFormOpen && selectedProductId && selectedProduct) {
      setFormData({
        name: selectedProduct.name,
        description: selectedProduct.description,
        inStock: selectedProduct.inStock,
        isFeatured: selectedProduct.isFeatured,
        brandId: selectedProduct.brand.id,
        categoryIds: selectedProduct.categories.map((c) => c.id),
        formImages: [],
        existingImages: selectedProduct.productImages.map((img) => ({
          id: img.id,
          isMain: img.isMain,
        })),
      });
      setSelectedCategoryIds(selectedProduct.categories.map((c) => ({ id: c.id, name: c.name })));
      setProductVariants(
        selectedProduct.productVariants.map((v: ProductVariant) => ({
          price: v.price,
          stockQuantity: v.stockQuantity,
          colorId: v.color.id,
          sizeId: v.size.id,
          formImages: [],
          existingImages: v.images.map((img) => ({
            id: img.id,
            isMain: img.isMain,
          })),
        }))
      );
      setDeletedImageIds([]);
      setDeletedVariantIds([]);
      setErrors({});
    } else if (isCreateFormOpen && !selectedProductId) {
      setFormData({
        name: "",
        description: "",
        inStock: true,
        isFeatured: false,
        brandId: "",
        categoryIds: [],
        formImages: [],
        existingImages: [],
      });
      setProductVariants([]);
      setSelectedCategoryIds([]);
      setDeletedImageIds([]);
      setDeletedVariantIds([]);
      setErrors({});
    }
  }, [isCreateFormOpen, selectedProductId, selectedProduct]);

  useEffect(() => {
    setSearch(params.searchTerm || "");
  }, [params.searchTerm]);

  const validateForm = () => {
    const newErrors: typeof errors = {};

    if (!formData.name?.trim()) {
      newErrors.name = "Product name is required.";
    }

    if (!formData.description?.trim()) {
      newErrors.description = "Product description is required.";
    }

    if (!formData.brandId) {
      newErrors.brandId = "Brand is required.";
    }

    if (formData.formImages.length > 0) {
      const maxSize = 5 * 1024 * 1024;
      const allowedTypes = ["image/jpeg", "image/png", "image/gif"];

      if (formData.formImages.some((file) => file.size === 0)) {
        newErrors.formImages = "All uploaded images must have content.";
      } else if (formData.formImages.some((file) => file.size > maxSize)) {
        newErrors.formImages = "Each image must be less than 5 MB.";
      } else if (formData.formImages.some((file) => !allowedTypes.includes(file.type))) {
        newErrors.formImages = "Only JPEG, PNG, and GIF images are allowed.";
      }
    }

    if (selectedProductId && formData.existingImages.length > 0) {
      if (
        formData.existingImages.some(
          (image) => !image.id || image.isMain === null || image.isMain === undefined
        )
      ) {
        newErrors.existingImages =
          "All existing images must have a valid ID and IsMain specified.";
      }
    }

    if (productVariants.length === 0) {
      newErrors.variants = "At least one variant is required.";
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const validateVariantForm = (variant: ProductVariantRequest) => {
    const newErrors: typeof variantErrors = {};

    if (!variant.sizeId) {
      newErrors.sizeId = "Size is required.";
    }

    if (!variant.colorId) {
      newErrors.colorId = "Color is required.";
    }

    if (variant.price <= 0) {
      newErrors.price = "Price must be greater than 0.";
    }

    if (variant.stockQuantity < 0) {
      newErrors.stockQuantity = "Stock quantity cannot be negative.";
    }

    if (variant.formImages.length > 0) {
      const maxSize = 5 * 1024 * 1024;
      const allowedTypes = ["image/jpeg", "image/png", "image/gif"];

      if (variant.formImages.some((file) => file.size === 0)) {
        newErrors.formImages = "All uploaded variant images must have content.";
      } else if (variant.formImages.some((file) => file.size > maxSize)) {
        newErrors.formImages = "Each variant image must be less than 5 MB.";
      } else if (variant.formImages.some((file) => !allowedTypes.includes(file.type))) {
        newErrors.formImages = "Only JPEG, PNG, and GIF images are allowed for variants.";
      }
    }

    return newErrors;
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
    setErrors((prev) => ({ ...prev, [name]: undefined }));
  };

  const handleSwitchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, checked } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: checked,
    }));
  };

  const handleCategorySelect = (categoryId: string, categoryName: string) => {
    setSelectedCategoryIds((prev) => {
      const isSelected = prev.some((item) => item.id === categoryId);
      if (isSelected) {
        return prev.filter((item) => item.id !== categoryId);
      } else {
        return [...prev, { id: categoryId, name: categoryName }];
      }
    });

    setFormData((prev) => ({
      ...prev,
      categoryIds: prev.categoryIds.includes(categoryId)
        ? prev.categoryIds.filter((id) => id !== categoryId)
        : [...prev.categoryIds, categoryId],
    }));
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files.length > 0) {
      const newFiles = Array.from(e.target.files);
      setFormData((prev) => ({
        ...prev,
        formImages: [...prev.formImages, ...newFiles],
      }));
      setErrors((prev) => ({ ...prev, formImages: undefined }));
    }
  };

  const handleVariantFileChange = (e: React.ChangeEvent<HTMLInputElement>, variantIndex?: number) => {
    if (e.target.files && e.target.files.length > 0) {
      const newFiles = Array.from(e.target.files);
      if (variantIndex !== undefined) {
        setProductVariants((prev) =>
          prev.map((v, i) =>
            i === variantIndex ? { ...v, formImages: [...v.formImages, ...newFiles] } : v
          )
        );
      } else {
        setNewVariantForm((prev) => ({
          ...prev,
          formImages: [...prev.formImages, ...newFiles],
        }));
      }
      setVariantErrors((prev) => ({ ...prev, formImages: undefined }));
    }
  };

  const handleDeleteUploadedImage = (index: number) => {
    setFormData((prev) => ({
      ...prev,
      formImages: prev.formImages.filter((_, i) => i !== index),
    }));
    setErrors((prev) => ({ ...prev, formImages: undefined }));
  };

  const handleDeleteVariantUploadedImage = (fileIndex: number, variantIndex?: number) => {
    if (variantIndex !== undefined) {
      setProductVariants((prev) =>
        prev.map((v, i) =>
          i === variantIndex
            ? { ...v, formImages: v.formImages.filter((_, fi) => fi !== fileIndex) }
            : v
        )
      );
    } else {
      setNewVariantForm((prev) => ({
        ...prev,
        formImages: prev.formImages.filter((_, i) => i !== fileIndex),
      }));
    }
    setVariantErrors((prev) => ({ ...prev, formImages: undefined }));
  };

  const handleDeleteExistingImage = (imageId: string) => {
    setDeletedImageIds((prev) => [...prev, imageId]);
    setFormData((prev) => ({
      ...prev,
      existingImages: prev.existingImages.filter((img) => img.id !== imageId),
    }));
    setErrors((prev) => ({ ...prev, existingImages: undefined }));
  };

  const handleDeleteExistingVariantImage = (variantIndex: number, imageId: string) => {
    setProductVariants((prev) =>
      prev.map((v, i) =>
        i === variantIndex
          ? {
              ...v,
              existingImages: v.existingImages.filter((img) => img.id !== imageId),
            }
          : v
      )
    );
  };

  const handleCopyId = (id: string) => {
    navigator.clipboard.writeText(id).then(() => {
      setNotification({
        open: true,
        message: "Product ID copied successfully",
        severity: "success",
      });
    }).catch((err) => {
      console.error("Failed to copy ID:", err);
      setNotification({
        open: true,
        message: "Failed to copy ID",
        severity: "error",
      });
    });
  };

  const handleBrandChange = (event: React.ChangeEvent<{ value: unknown }>) => {
    const brandId = event.target.value as string;
    setFormData((prev) => ({
      ...prev,
      brandId,
    }));
    setErrors((prev) => ({ ...prev, brandId: undefined }));
  };

  const handleVariantInputChange = (
    e: React.ChangeEvent<HTMLInputElement>,
    variantIndex?: number
  ) => {
    const { name, value } = e.target;
    const numericValue = value === "" ? 0 : parseFloat(value);
    if (variantIndex !== undefined) {
      setProductVariants((prev) =>
        prev.map((v, i) => (i === variantIndex ? { ...v, [name]: numericValue } : v))
      );
    } else {
      setNewVariantForm((prev) => ({
        ...prev,
        [name]: numericValue,
      }));
    }
    setVariantErrors((prev) => ({ ...prev, [name]: undefined }));
  };

  const handleVariantSelectChange = (
    name: string,
    value: string,
    variantIndex?: number
  ) => {
    if (variantIndex !== undefined) {
      setProductVariants((prev) =>
        prev.map((v, i) => (i === variantIndex ? { ...v, [name]: value } : v))
      );
    } else {
      setNewVariantForm((prev) => ({
        ...prev,
        [name]: value,
      }));
    }
    setVariantErrors((prev) => ({ ...prev, [name]: undefined }));
  };

  const handleAddVariant = () => {
    const newErrors = validateVariantForm(newVariantForm);
    if (Object.keys(newErrors).length > 0) {
      setVariantErrors(newErrors);
      setNotification({
        open: true,
        message: "Please fix the errors in the new variant form.",
        severity: "error",
      });
      return;
    }

    const newVariant: ProductVariantRequest = {
      price: newVariantForm.price,
      stockQuantity: newVariantForm.stockQuantity,
      colorId: newVariantForm.colorId,
      sizeId: newVariantForm.sizeId,
      formImages: newVariantForm.formImages,
      existingImages: [],
    };

    setProductVariants((prev) => [...prev, newVariant]);
    setNewVariantForm({
      price: 0,
      stockQuantity: 0,
      colorId: "",
      sizeId: "",
      formImages: [],
      existingImages: [],
    });
    setVariantErrors({});
    setErrors((prev) => ({ ...prev, variants: undefined }));
  };

  const handleDeleteVariant = (variantIndex: number) => {
    if (selectedProduct && selectedProduct.productVariants[variantIndex]?.id) {
      setDeletedVariantIds((prev) => [...prev, selectedProduct.productVariants[variantIndex].id]);
    }
    setProductVariants((prev) => prev.filter((_, i) => i !== variantIndex));
  };

  const buildProductFormData = () => {
    const productFormData = new FormData();
    productFormData.append("Name", formData.name);
    productFormData.append("Description", formData.description);
    productFormData.append("InStock", formData.inStock.toString());
    productFormData.append("IsFeatured", formData.isFeatured.toString());
    productFormData.append("BrandId", formData.brandId);

    formData.categoryIds.forEach((categoryId, index) => {
      productFormData.append(`CategoryIds[${index}]`, categoryId);
    });

    formData.formImages.forEach((file) => {
      productFormData.append("FormImages", file);
    });

    formData.existingImages.forEach((image, index) => {
      productFormData.append(`Images[${index}].Id`, image.id);
      productFormData.append(`Images[${index}].IsMain`, image.isMain.toString());
    });

    return productFormData;
  };

  const buildVariantFormData = (variant: ProductVariantRequest, productId: string) => {
    const variantFormData = new FormData();
    variantFormData.append("Price", variant.price.toString());
    variantFormData.append("StockQuantity", variant.stockQuantity.toString());
    variantFormData.append("ProductId", productId);
    variantFormData.append("ColorId", variant.colorId);
    variantFormData.append("SizeId", variant.sizeId);

    variant.formImages.forEach((file) => {
      variantFormData.append("FormImages", file);
    });

    variant.existingImages.forEach((image, index) => {
      variantFormData.append(`Images[${index}].Id`, image.id);
      variantFormData.append(`Images[${index}].IsMain`, image.isMain.toString());
    });

    return variantFormData;
  };

  const handleSaveProduct = async () => {
    if (!validateForm()) {
      setNotification({
        open: true,
        message: "Please fix the errors in the form before saving.",
        severity: "error",
      });
      return;
    }

    const variantValidationErrors = productVariants
      .map((variant, index) => ({
        index,
        errors: validateVariantForm(variant),
      }))
      .filter((v) => Object.keys(v.errors).length > 0);

    if (variantValidationErrors.length > 0) {
      setNotification({
        open: true,
        message: "Please fix the errors in the variant forms.",
        severity: "error",
      });
      return;
    }

    try {
      const productFormData = buildProductFormData();
      let product: Product;

      if (selectedProductId) {
        product = await updateProduct({ id: selectedProductId, data: productFormData }).unwrap();

        for (const variantId of deletedVariantIds) {
          await deleteProductVariant(variantId).unwrap();
        }

        for (let i = 0; i < productVariants.length; i++) {
          const variant = productVariants[i];
          const variantFormData = buildVariantFormData(variant, product.id);
          const originalVariantId = selectedProduct?.productVariants[i]?.id;

          if (originalVariantId && !deletedVariantIds.includes(originalVariantId)) {
            await updateProductVariant({
              id: originalVariantId,
              data: variantFormData,
            }).unwrap();
          } else {
            await createProductVariant(variantFormData).unwrap();
          }
        }

        setNotification({
          open: true,
          message: "Product and variants updated successfully",
          severity: "success",
        });
      } else {
        product = await createProduct(productFormData).unwrap();

        for (const variant of productVariants) {
          const variantFormData = buildVariantFormData(variant, product.id);
          await createProductVariant(variantFormData).unwrap();
        }

        setNotification({
          open: true,
          message: "Product and variants created successfully",
          severity: "success",
        });
      }

      handleCloseForm();
    } catch (err) {
      console.error("Failed to save product or variants:", err);
      setNotification({
        open: true,
        message: "Failed to save product or variants",
        severity: "error",
      });
    }
  };

  const handleConfirmDelete = async () => {
    if (selectedProductId) {
      try {
        await deleteProduct(selectedProductId).unwrap();
        dispatch(setSelectedProductId(null));
        setSelectedProductIds((prev) => prev.filter((id) => id !== selectedProductId));
        setNotification({
          open: true,
          message: "Product deleted successfully",
          severity: "success",
        });
        handleCloseDeleteDialog();
      } catch (err) {
        console.error("Failed to delete product:", err);
        setNotification({
          open: true,
          message: "Failed to delete product",
          severity: "error",
        });
      }
    }
  };

  const handleDeleteAllSelected = async () => {
    try {
      for (const id of selectedProductIds) {
        await deleteProduct(id).unwrap();
      }
      setSelectedProductIds([]);
      setNotification({
        open: true,
        message: `${selectedProductIds.length} product${selectedProductIds.length > 1 ? "s" : ""} deleted successfully`,
        severity: "success",
      });
    } catch (err) {
      console.error("Failed to delete products:", err);
      setNotification({
        open: true,
        message: "Failed to delete some products",
        severity: "error",
      });
    }
  };

  const handleCheckboxChange = (id: string) => {
    setSelectedProductIds((prev) =>
      prev.includes(id) ? prev.filter((pid) => pid !== id) : [...prev, id]
    );
  };

  const handleSelectAllChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    if (event.target.checked) {
      const allProductIds = data?.items?.map((product: Product) => product.id) || [];
      setSelectedProductIds(allProductIds);
    } else {
      setSelectedProductIds([]);
    }
  };

  const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    setSearch(value);
    debouncedSearch(value);
  };

  const handleClearSearch = () => {
    setSearch("");
    dispatch(setParams({ searchTerm: undefined }));
    dispatch(setPageNumber(1));
  };

  const handlePageChange = (_event: React.ChangeEvent<unknown>, page: number) => {
    dispatch(setPageNumber(page));
  };

  const handleCreateClick = () => {
    dispatch(setCreateFormOpen(true));
    dispatch(setSelectedProductId(null));
  };

  const handleEditClick = (product: Product) => {
    dispatch(setSelectedProductId(product.id));
    dispatch(setCreateFormOpen(true));
  };

  const handleDeleteClick = (id: string) => {
    dispatch(setSelectedProductId(id));
    dispatch(setDeleteDialogOpen(true));
  };

  const handleCloseForm = () => {
    dispatch(setCreateFormOpen(false));
    dispatch(setSelectedProductId(null));
    setFormData({
      name: "",
      description: "",
      inStock: true,
      isFeatured: false,
      brandId: "",
      categoryIds: [],
      formImages: [],
      existingImages: [],
    });
    setProductVariants([]);
    setDeletedImageIds([]);
    setDeletedVariantIds([]);
    setSelectedCategoryIds([]);
    setAnchorEl(null);
    setErrors({});
    setNewVariantForm({
      price: 0,
      stockQuantity: 0,
      colorId: "",
      sizeId: "",
      formImages: [],
      existingImages: [],
    });
    setVariantErrors({});
  };

  const handleCloseDeleteDialog = () => {
    dispatch(setDeleteDialogOpen(false));
    dispatch(setSelectedProductId(null));
  };

  const handleCloseNotification = () => {
    setNotification({ ...notification, open: false });
  };

  const calculateStartIndex = (pagination: PaginationParams | undefined) => {
    if (!pagination) return 0;
    return (pagination.currentPage - 1) * pagination.pageSize + 1;
  };

  const calculateEndIndex = (pagination: PaginationParams | undefined) => {
    if (!pagination) return 0;
    const endIndex = pagination.currentPage * pagination.pageSize;
    return endIndex > pagination.totalCount ? pagination.totalCount : endIndex;
  };

  const getTotalStock = (product: Product): number => {
    return product.productVariants.reduce((total: number, variant: ProductVariant) => total + variant.stockQuantity, 0);
  };

  const getVariantCount = (product: Product): number => {
    return product.productVariants.length;
  };

  if (isLoading) {
    return (
      <Box sx={{ display: "flex", justifyContent: "center", padding: 5 }}>
        <CircularProgress />
        <Typography variant="h6" sx={{ marginLeft: 2 }}>
          Loading products...
        </Typography>
      </Box>
    );
  }

  if (error) {
    return (
      <Box sx={{ padding: 3, margin: 2, backgroundColor: "#fff" }}>
        <Typography variant="h6" color="error">
          Error loading products
        </Typography>
        <Typography variant="body2" sx={{ marginTop: 1 }}>
          Please try again later or contact support.
        </Typography>
        <Button
          startIcon={<RefreshIcon />}
          variant="outlined"
          color="primary"
          sx={{ marginTop: 2 }}
          onClick={() => refetch()}
        >
          Retry
        </Button>
      </Box>
    );
  }

  return (
    <Box sx={{ padding: 3, margin: "auto" }}>
      <Snackbar
        open={notification.open}
        autoHideDuration={6000}
        onClose={handleCloseNotification}
        anchorOrigin={{
          vertical: "top",
          horizontal: "right",
        }}
      >
        <Alert
          onClose={handleCloseNotification}
          severity={notification.severity}
          sx={{ width: "100%" }}
        >
          {notification.message}
        </Alert>
      </Snackbar>

      <Box sx={{ marginBottom: 4 }}>
        <Typography variant="h4" sx={{ fontWeight: "bold", marginBottom: 1 }}>
          Product Management
        </Typography>
        <Typography variant="body1">Manage your products and their variants</Typography>
      </Box>

      <Paper sx={{ elevation: 2, padding: 3, marginBottom: 4 }}>
        <Box
          sx={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
            marginBottom: 3,
          }}
        >
          <TextField
            placeholder="Search products..."
            value={search}
            onChange={handleSearchChange}
            variant="outlined"
            size="small"
            sx={{ width: "300px", "& input": { paddingLeft: 4 } }}
            InputProps={{
              startAdornment: (
                <InputAdornment position="start">
                  <SearchIcon />
                </InputAdornment>
              ),
              endAdornment: search && (
                <InputAdornment position="end">
                  <IconButton size="small" onClick={handleClearSearch}>
                    <ClearIcon />
                  </IconButton>
                </InputAdornment>
              ),
            }}
            disabled={isFetching}
          />
          <Box sx={{ display: "flex", gap: 1 }}>
            {selectedProductIds.length > 0 && (
              <Button
                variant="contained"
                color="error"
                onClick={handleDeleteAllSelected}
                sx={{ borderRadius: "12px", textTransform: "inherit" }}
                disabled={isDeleting}
              >
                Delete Selected Items ({selectedProductIds.length})
              </Button>
            )}
            <Button
              variant="contained"
              color="primary"
              onClick={handleCreateClick}
              startIcon={<AddCircleIcon />}
              sx={{ borderRadius: "12px", textTransform: "inherit" }}
            >
              Add Product
            </Button>
          </Box>
        </Box>

        <TableContainer
          component={Paper}
          elevation={0}
          sx={{ marginBottom: 2, border: 1, borderColor: "grey.300", borderRadius: 1 }}
        >
          <Table sx={{ minWidth: 650 }}>
            <TableHead>
              <TableRow>
                <TableCell padding="checkbox">
                  <Checkbox
                    checked={
                      (data?.items?.length ?? 0) > 0 &&
                      selectedProductIds.length === data?.items.length
                    }
                    indeterminate={
                      selectedProductIds.length > 0 &&
                      selectedProductIds.length < (data?.items?.length ?? 0)
                    }
                    onChange={handleSelectAllChange}
                  />
                </TableCell>
                <TableCell>Product Name</TableCell>
                <TableCell>Brand</TableCell>
                <TableCell>Price Range</TableCell>
                <TableCell>Variants</TableCell>
                <TableCell align="center">Stock Status</TableCell>
                <TableCell align="center">Total Stock</TableCell>
                <TableCell align="center">Rating</TableCell>
                <TableCell>Categories</TableCell>
                <TableCell align="center">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {data?.items.map((product: Product) => (
                <TableRow key={product.id}>
                  <TableCell padding="checkbox">
                    <Checkbox
                      checked={selectedProductIds.includes(product.id)}
                      onChange={() => handleCheckboxChange(product.id)}
                    />
                  </TableCell>
                  <TableCell>
                    <Tooltip title={product.name}>
                      <Box sx={{ display: "flex", alignItems: "center" }}>
                        {product.productImages && product.productImages.length > 0 ? (
                          <Box
                            component="img"
                            src={product.productImages[0].imageUrl}
                            alt={product.name}
                            sx={{
                              width: 40,
                              height: 40,
                              marginRight: 1,
                              objectFit: "cover",
                              borderRadius: "4px",
                            }}
                          />
                        ) : (
                          <Box
                            sx={{
                              width: 40,
                              height: 40,
                              marginRight: 1,
                              display: "flex",
                              alignItems: "center",
                              justifyContent: "center",
                              backgroundColor: "#f0f0f0",
                              borderRadius: "4px",
                            }}
                          >
                            <InventoryIcon color="disabled" fontSize="small" />
                          </Box>
                        )}
                        <Box>
                          <Typography
                            variant="body1"
                            sx={{
                              fontWeight: "medium",
                              maxWidth: "200px",
                              overflow: "hidden",
                              textOverflow: "ellipsis",
                              whiteSpace: "nowrap",
                            }}
                          >
                            {product.name}
                          </Typography>
                          <Typography
                            variant="body2"
                            color="textSecondary"
                            sx={{
                              maxWidth: "200px",
                              overflow: "hidden",
                              textOverflow: "ellipsis",
                              whiteSpace: "nowrap",
                            }}
                          >
                            {product.description}
                          </Typography>
                        </Box>
                      </Box>
                    </Tooltip>
                  </TableCell>
                  <TableCell>
                    <Tooltip title={product.brand.description || "N/A"}>
                      <Typography
                        variant="body2"
                        sx={{
                          maxWidth: "150px",
                          overflow: "hidden",
                          textOverflow: "ellipsis",
                          whiteSpace: "nowrap",
                        }}
                      >
                        {product.brand.name}
                      </Typography>
                    </Tooltip>
                  </TableCell>
                  <TableCell>
                    <Typography variant="body2" fontWeight={500}>
                      {formatVND(product.minPrice)} - {formatVND(product.maxPrice)}
                    </Typography>
                  </TableCell>
                  <TableCell>
                    <Button
                      variant="outlined"
                      size="small"
                      onClick={() => setViewingVariants(product)}
                      startIcon={<EyeIcon />}
                      sx={{ textTransform: "none" }}
                    >
                      {getVariantCount(product)} variants
                    </Button>
                  </TableCell>
                  <TableCell>
                    <Chip
                      label={product.inStock ? "In Stock" : "Out of Stock"}
                      size="small"
                      color={product.inStock ? "success" : "error"}
                      variant="outlined"
                    />
                  </TableCell>
                  <TableCell align="center">
                    <Typography variant="body2">{getTotalStock(product)}</Typography>
                  </TableCell>
                  <TableCell>
                    <Box sx={{ display: "flex", alignItems: "center" }}>
                      <Rating
                        value={product.averageRating}
                        precision={0.5}
                        size="small"
                        readOnly
                      />
                      <Typography variant="body2" sx={{ marginLeft: 1 }}>
                        ({product.averageRating.toFixed(1)})
                      </Typography>
                    </Box>
                  </TableCell>
                  <TableCell>
                    <Box sx={{ display: "flex", flexWrap: "wrap", gap: 0.5 }}>
                      {product.categories.map((category) => (
                        <Chip
                          key={category.id}
                          label={category.name}
                          size="small"
                          sx={{ fontSize: "0.75rem" }}
                          color={category.subCategories.length > 0 ? "error" : "success"}
                        />
                      ))}
                    </Box>
                  </TableCell>
                  <TableCell align="center">
                    <Box sx={{ display: "flex", justifyContent: "center", gap: 1 }}>
                      <Tooltip title="Copy ID">
                        <IconButton
                          size="small"
                          color="inherit"
                          onClick={() => handleCopyId(product.id)}
                        >
                          <ContentCopyIcon fontSize="small" />
                        </IconButton>
                      </Tooltip>
                      <Tooltip title="Edit">
                        <IconButton
                          size="small"
                          color="primary"
                          onClick={() => handleEditClick(product)}
                        >
                          <EditIcon fontSize="small" />
                        </IconButton>
                      </Tooltip>
                      <Tooltip title="View Variants">
                        <IconButton
                          size="small"
                          color="primary"
                          onClick={() => setViewingVariants(product)}
                        >
                          <EyeIcon fontSize="small" />
                        </IconButton>
                      </Tooltip>
                      <Tooltip title="Delete">
                        <IconButton
                          size="small"
                          color="error"
                          onClick={() => handleDeleteClick(product.id)}
                        >
                          <DeleteIcon fontSize="small" />
                        </IconButton>
                      </Tooltip>
                    </Box>
                  </TableCell>
                </TableRow>
              ))}

              {(!data?.items || data.items.length === 0) && (
                <TableRow>
                  <TableCell colSpan={9} align="center" sx={{ paddingY: 3 }}>
                    <Typography variant="body1" color="textSecondary">
                      {search ? `No products found for "${search}"` : "No products found"}
                    </Typography>
                    {search && (
                      <Button
                        startIcon={<ClearIcon />}
                        onClick={handleClearSearch}
                        sx={{ marginTop: 2 }}
                      >
                        Clear Search
                      </Button>
                    )}
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </TableContainer>

        {data?.pagination && data.items.length > 0 && (
          <Box
            sx={{
              display: "flex",
              justifyContent: "space-between",
              alignItems: "center",
              marginTop: 3,
            }}
          >
            <Typography variant="body2" color="textSecondary">
              Showing {calculateStartIndex(data.pagination)} - {calculateEndIndex(data.pagination)} of{" "}
              {data.pagination.totalCount} products
            </Typography>
            <Pagination
              count={data.pagination.totalPages}
              page={data.pagination.currentPage}
              onChange={handlePageChange}
              color="primary"
              shape="rounded"
            />
          </Box>
        )}
      </Paper>

      <Dialog open={isCreateFormOpen} onClose={handleCloseForm} fullWidth maxWidth="md">
        <DialogTitle>
          {selectedProductId ? "Edit Product" : "Create New Product"}
          <IconButton
            aria-label="close"
            onClick={handleCloseForm}
            sx={{ position: "absolute", right: 8, top: 8 }}
          >
            <CloseIcon />
          </IconButton>
        </DialogTitle>
        <DialogContent dividers>
          {isLoadingProduct || isLoadingCategories || isLoadingBrands || isLoadingSizes || isLoadingColors ? (
            <Box sx={{ display: "flex", justifyContent: "center", padding: 3 }}>
              <CircularProgress />
            </Box>
          ) : (
            <Grid container spacing={3}>
              {selectedProductId && (
                <Grid size={{ xs: 12 }}>
                  <TextField
                    label="Product ID"
                    fullWidth
                    value={selectedProductId}
                    margin="normal"
                    InputProps={{
                      readOnly: true,
                    }}
                  />
                </Grid>
              )}
              <Grid size={{ xs: 12, sm: 6 }}>
                <TextField
                  name="name"
                  label="Product Name"
                  fullWidth
                  required
                  value={formData.name || ""}
                  onChange={handleInputChange}
                  margin="normal"
                  error={!!errors.name}
                  helperText={errors.name}
                />
              </Grid>
              <Grid size={{ xs: 12, sm: 6 }}>
                <FormControl fullWidth margin="normal" error={!!errors.brandId}>
                  <InputLabel id="brand-label">Brand</InputLabel>
                  <Select
                    labelId="brand-label"
                    label="Brand"
                    value={formData.brandId || ""}
                    onChange={handleBrandChange}
                  >
                    {brandsData?.map((brand: Brand) => (
                      <MenuItem key={brand.id} value={brand.id}>
                        {brand.name}
                      </MenuItem>
                    ))}
                  </Select>
                  {errors.brandId && (
                    <Typography variant="body2" color="error">
                      {errors.brandId}
                    </Typography>
                  )}
                </FormControl>
              </Grid>
              <Grid size={{ xs: 12 }}>
                <TextField
                  name="description"
                  label="Description"
                  fullWidth
                  multiline
                  rows={4}
                  value={formData.description || ""}
                  onChange={handleInputChange}
                  margin="normal"
                  error={!!errors.description}
                  helperText={errors.description}
                />
              </Grid>
              <Grid size={{ xs: 12, sm: 6 }}>
                <FormControl fullWidth margin="normal">
                  <InputLabel id="categories-label">Categories</InputLabel>
                  <Select
                    labelId="categories-label"
                    open={Boolean(anchorEl)}
                    onOpen={(event) => setAnchorEl(event.currentTarget as HTMLElement)}
                    onClose={() => setAnchorEl(null)}
                    value={selectedCategoryIds.map((item) => item.id)}
                    multiple
                    renderValue={() => (
                      <Box sx={{ display: "flex", flexWrap: "wrap", gap: 0.5 }}>
                        {selectedCategoryIds.map((category) => (
                          <Chip
                            key={category.id}
                            label={category.name}
                            size="small"
                            sx={{ backgroundColor: "grey.200", color: "text.primary" }}
                          />
                        ))}
                      </Box>
                    )}
                    MenuProps={{
                      PaperProps: {
                        sx: { minWidth: 200 },
                      },
                    }}
                  >
                    <CategoryMenu
                      categories={categoriesData || []}
                      depth={0}
                      selectedCategoryIds={selectedCategoryIds}
                      onSelect={handleCategorySelect}
                    />
                  </Select>
                </FormControl>
              </Grid>
              <Grid size={{ xs: 12, sm: 6 }}>
                <FormControlLabel
                  control={
                    <Switch
                      name="inStock"
                      checked={formData.inStock || false}
                      onChange={handleSwitchChange}
                      color="primary"
                    />
                  }
                  label="In Stock"
                />
                <FormControlLabel
                  control={
                    <Switch
                      name="isFeatured"
                      checked={formData.isFeatured || false}
                      onChange={handleSwitchChange}
                      color="primary"
                    />
                  }
                  label="Featured"
                />
              </Grid>
              <Grid size={{ xs: 12 }}>
                <Button
                  variant="outlined"
                  component="label"
                  startIcon={<UploadIcon />}
                  sx={{ marginTop: 2 }}
                >
                  Upload Product Images
                  <input
                    type="file"
                    hidden
                    multiple
                    accept="image/jpeg,image/png,image/gif"
                    onChange={handleFileChange}
                  />
                </Button>
                {errors.formImages && (
                  <Typography variant="body2" color="error" sx={{ marginTop: 1 }}>
                    {errors.formImages}
                  </Typography>
                )}
                {formData.formImages.length > 0 && (
                  <Box sx={{ marginTop: 2 }}>
                    <Typography variant="body2">Uploaded Product Images:</Typography>
                    <Box sx={{ display: "flex", flexWrap: "wrap", gap: 1, marginTop: 1 }}>
                      {formData.formImages.map((file, index) => (
                        <Box key={index} sx={{ position: "relative" }}>
                          <Box
                            component="img"
                            src={previewUrls[index]}
                            alt={file.name}
                            sx={{
                              width: 80,
                              height: 80,
                              objectFit: "cover",
                              borderRadius: 1,
                            }}
                          />
                          <IconButton
                            size="small"
                            color="error"
                            onClick={() => handleDeleteUploadedImage(index)}
                            sx={{
                              position: "absolute",
                              top: 0,
                              right: 0,
                              backgroundColor: "rgba(255, 255, 255, 0.7)",
                            }}
                          >
                            <DeleteIcon fontSize="small" />
                          </IconButton>
                        </Box>
                      ))}
                    </Box>
                  </Box>
                )}
                {selectedProductId && formData.existingImages.length > 0 && (
                  <Box sx={{ marginTop: 2 }}>
                    <Typography variant="body2">Current Product Images:</Typography>
                    {errors.existingImages && (
                      <Typography variant="body2" color="error" sx={{ marginTop: 1 }}>
                        {errors.existingImages}
                      </Typography>
                    )}
                    <Box sx={{ display: "flex", flexWrap: "wrap", gap: 1, marginTop: 1 }}>
                      {formData.existingImages.map((image) => (
                        <Box key={image.id} sx={{ position: "relative" }}>
                          <Box
                            component="img"
                            src={
                              selectedProduct?.productImages.find((img) => img.id === image.id)
                                ?.imageUrl
                            }
                            alt="Product"
                            sx={{
                              width: 80,
                              height: 80,
                              objectFit: "cover",
                              borderRadius: 1,
                              border: image.isMain ? "2px solid #1976d2" : "none",
                            }}
                          />
                          <IconButton
                            size="small"
                            color="error"
                            onClick={() => handleDeleteExistingImage(image.id)}
                            sx={{
                              position: "absolute",
                              top: 0,
                              right: 0,
                              backgroundColor: "rgba(255, 255, 255, 0.7)",
                            }}
                          >
                            <DeleteIcon fontSize="small" />
                          </IconButton>
                        </Box>
                      ))}
                    </Box>
                  </Box>
                )}
              </Grid>
              <Grid size={{ xs: 12 }}>
                <Typography variant="h6" sx={{ marginTop: 2, marginBottom: 1 }}>
                  Product Variants
                </Typography>
                {errors.variants && (
                  <Typography variant="body2" color="error" sx={{ marginBottom: 1 }}>
                    {errors.variants}
                  </Typography>
                )}
                {productVariants.length > 0 && (
                  <TableContainer component={Paper} elevation={0}>
                    <Table>
                      <TableHead>
                        <TableRow>
                          <TableCell>Size</TableCell>
                          <TableCell>Color</TableCell>
                          <TableCell>Price</TableCell>
                          <TableCell>Stock</TableCell>
                          <TableCell>Images</TableCell>
                          <TableCell align="center">Actions</TableCell>
                        </TableRow>
                      </TableHead>
                      <TableBody>
                        {productVariants.map((variant, index) => {
                          const variantErrors = validateVariantForm(variant);
                          const variantPreviewUrls = variant.formImages.map((file) =>
                            URL.createObjectURL(file)
                          );
                          return (
                            <TableRow key={index}>
                              <TableCell>
                                <FormControl
                                  fullWidth
                                  margin="normal"
                                  error={!!variantErrors.sizeId}
                                >
                                  <InputLabel id={`variant-size-label-${index}`}>Size</InputLabel>
                                  <Select
                                    labelId={`variant-size-label-${index}`}
                                    label="Size"
                                    value={variant.sizeId || ""}
                                    onChange={(event) => {
                                      handleVariantSelectChange("sizeId", event.target.value, index);
                                    }}
                                  >
                                    {sizesData?.map((size: Size) => (
                                      <MenuItem key={size.id} value={size.id}>
                                        {size.name}
                                      </MenuItem>
                                    ))}
                                  </Select>
                                  {variantErrors.sizeId && (
                                    <Typography variant="body2" color="error">
                                      {variantErrors.sizeId}
                                    </Typography>
                                  )}
                                </FormControl>
                              </TableCell>
                              <TableCell>
                                <FormControl
                                  fullWidth
                                  margin="normal"
                                  error={!!variantErrors.colorId}
                                >
                                  <InputLabel id={`variant-color-label-${index}`}>
                                    Color
                                  </InputLabel>
                                  <Select
                                    labelId={`variant-color-label-${index}`}
                                    label="Color"
                                    value={variant.colorId || ""}
                                    onChange={(event) => {
                                      handleVariantSelectChange("colorId", event.target.value, index);
                                    }}
                                  >
                                    {colorsData?.map((color: Color) => (
                                      <MenuItem key={color.id} value={color.id}>
                                        {color.name}
                                      </MenuItem>
                                    ))}
                                  </Select>
                                  {variantErrors.colorId && (
                                    <Typography variant="body2" color="error">
                                      {variantErrors.colorId}
                                    </Typography>
                                  )}
                                </FormControl>
                              </TableCell>
                              <TableCell>
                                <TextField
                                  name="price"
                                  label="Price (VND)"
                                  type="number"
                                  fullWidth
                                  value={variant.price || 0}
                                  onChange={(e) => handleVariantInputChange(e as React.ChangeEvent<HTMLInputElement>, index)}
                                  margin="normal"
                                  InputProps={{
                                    endAdornment: (
                                      <InputAdornment position="end">₫</InputAdornment>
                                    ),
                                    inputProps: { step: 1, min: 0 },
                                  }}
                                  error={!!variantErrors.price}
                                  helperText={variantErrors.price}
                                />
                              </TableCell>
                              <TableCell>
                                <TextField
                                  name="stockQuantity"
                                  label="Stock"
                                  type="number"
                                  fullWidth
                                  value={variant.stockQuantity || 0}
                                  onChange={(e) => handleVariantInputChange(e as React.ChangeEvent<HTMLInputElement>, index)}
                                  margin="normal"
                                  InputProps={{
                                    inputProps: { step: 1, min: 0 },
                                  }}
                                  error={!!variantErrors.stockQuantity}
                                  helperText={variantErrors.stockQuantity}
                                />
                              </TableCell>
                              <TableCell>
                                <Box sx={{ display: "flex", flexWrap: "wrap", gap: 1 }}>
                                  {variant.existingImages.map((image) => (
                                    <Box key={image.id} sx={{ position: "relative" }}>
                                      <Box
                                        component="img"
                                        src={
                                          selectedProduct?.productVariants[index]?.images.find(
                                            (img) => img.id === image.id
                                          )?.imageUrl
                                        }
                                        alt="variant"
                                        sx={{
                                          width: 40,
                                          height: 40,
                                          objectFit: "cover",
                                          borderRadius: 1,
                                          border: image.isMain ? "2px solid #1976d2" : "none",
                                        }}
                                      />
                                      <IconButton
                                        size="small"
                                        color="error"
                                        onClick={() =>
                                          handleDeleteExistingVariantImage(index, image.id)
                                        }
                                        sx={{
                                          position: "absolute",
                                          top: 0,
                                          right: 0,
                                          backgroundColor: "rgba(255, 255, 255, 0.7)",
                                        }}
                                      >
                                        <DeleteIcon fontSize="small" />
                                      </IconButton>
                                    </Box>
                                  ))}
                                  {variant.formImages.map((file, i) => (
                                    <Box key={`form-${i}`} sx={{ position: "relative" }}>
                                      <Box
                                        component="img"
                                        src={variantPreviewUrls[i]}
                                        alt={file.name}
                                        sx={{
                                          width: 40,
                                          height: 40,
                                          objectFit: "cover",
                                          borderRadius: 1,
                                        }}
                                      />
                                      <IconButton
                                        size="small"
                                        color="error"
                                        onClick={() => handleDeleteVariantUploadedImage(i, index)}
                                        sx={{
                                          position: "absolute",
                                          top: 0,
                                          right: 0,
                                          backgroundColor: "rgba(255, 255, 255, 0.7)",
                                        }}
                                      >
                                        <DeleteIcon fontSize="small" />
                                      </IconButton>
                                    </Box>
                                  ))}
                                  {variant.existingImages.length === 0 &&
                                    variant.formImages.length === 0 && (
                                      <Typography variant="body2">No images</Typography>
                                    )}
                                </Box>
                                <Button
                                  variant="outlined"
                                  component="label"
                                  startIcon={<UploadIcon />}
                                  sx={{ marginTop: 1 }}
                                >
                                  Upload
                                  <input
                                    type="file"
                                    hidden
                                    multiple
                                    accept="image/jpeg,image/png,image/gif"
                                    onChange={(e) => handleVariantFileChange(e, index)}
                                  />
                                </Button>
                                {variantErrors.formImages && (
                                  <Typography
                                    variant="body2"
                                    color="error"
                                    sx={{ marginTop: 1 }}
                                  >
                                    {variantErrors.formImages}
                                  </Typography>
                                )}
                              </TableCell>
                              <TableCell align="center">
                                <IconButton
                                  size="small"
                                  color="error"
                                  onClick={() => handleDeleteVariant(index)}
                                >
                                  <DeleteIcon fontSize="small" />
                                </IconButton>
                              </TableCell>
                            </TableRow>
                          );
                        })}
                      </TableBody>
                    </Table>
                  </TableContainer>
                )}
                <Box
                  sx={{
                    marginTop: 2,
                    padding: 2,
                    backgroundColor: "grey.100",
                    borderRadius: 1,
                  }}
                >
                  <Typography variant="h6" sx={{ marginBottom: 1 }}>
                    Add New Variant
                  </Typography>
                  <Grid container spacing={2}>
                    <Grid size={{ xs: 12, sm: 3 }}>
                      <FormControl fullWidth margin="normal" error={!!variantErrors.sizeId}>
                        <InputLabel id="new-variant-size-label">Size</InputLabel>
                        <Select
                          labelId="new-variant-size-label"
                          label="Size"
                          value={newVariantForm.sizeId || ""}
                          onChange={(event) => {
                            handleVariantSelectChange("sizeId", event.target.value);
                          }}
                        >
                          {sizesData?.map((size: Size) => (
                            <MenuItem key={size.id} value={size.id}>
                              {size.name}
                            </MenuItem>
                          ))}
                        </Select>
                        {variantErrors.sizeId && (
                          <Typography variant="body2" color="error">
                            {variantErrors.sizeId}
                          </Typography>
                        )}
                      </FormControl>
                    </Grid>
                    <Grid size={{ xs: 12, sm: 3 }}>
                      <FormControl fullWidth margin="normal" error={!!variantErrors.colorId}>
                        <InputLabel id="new-variant-color-label">Color</InputLabel>
                        <Select
                          labelId="new-variant-color-label"
                          label="Color"
                          value={newVariantForm.colorId || ""}
                          onChange={(event) => {
                            handleVariantSelectChange("colorId", event.target.value);
                          }}
                        >
                          {colorsData?.map((color: Color) => (
                            <MenuItem key={color.id} value={color.id}>
                              {color.name}
                            </MenuItem>
                          ))}
                        </Select>
                        {variantErrors.colorId && (
                          <Typography variant="body2" color="error">
                            {variantErrors.colorId}
                          </Typography>
                        )}
                      </FormControl>
                    </Grid>
                    <Grid size={{ xs: 12, sm: 2 }}>
                      <TextField
                        name="price"
                        label="Price (VND)"
                        type="number"
                        fullWidth
                        value={newVariantForm.price || 0}
                        onChange={handleVariantInputChange}
                        margin="normal"
                        InputProps={{
                          endAdornment: <InputAdornment position="end">₫</InputAdornment>,
                          inputProps: { step: 1, min: 0 },
                        }}
                        error={!!variantErrors.price}
                        helperText={variantErrors.price}
                      />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 2 }}>
                      <TextField
                        name="stockQuantity"
                        label="Stock"
                        type="number"
                        fullWidth
                        value={newVariantForm.stockQuantity || 0}
                        onChange={handleVariantInputChange}
                        margin="normal"
                        InputProps={{
                          inputProps: { step: 1, min: 0 },
                        }}
                        error={!!variantErrors.stockQuantity}
                        helperText={variantErrors.stockQuantity}
                      />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 2 }}>
                      <Button
                        variant="contained"
                        onClick={handleAddVariant}
                        sx={{ marginTop: 2, textTransform: "none" }}
                        fullWidth
                      >
                        Add Variant
                      </Button>
                    </Grid>
                    <Grid size={{ xs: 12 }}>
                      <Button
                        variant="outlined"
                        component="label"
                        startIcon={<UploadIcon />}
                        sx={{ marginTop: 1 }}
                      >
                        Upload Variant Images
                        <input
                          type="file"
                          hidden
                          multiple
                          accept="image/jpeg,image/png,image/gif"
                          onChange={handleVariantFileChange}
                        />
                      </Button>
                      {variantErrors.formImages && (
                        <Typography variant="body2" color="error" sx={{ marginTop: 1 }}>
                          {variantErrors.formImages}
                        </Typography>
                      )}
                      {newVariantForm.formImages.length > 0 && (
                        <Box sx={{ marginTop: 2 }}>
                          <Typography variant="body2">Uploaded Variant Images:</Typography>
                          <Box
                            sx={{ display: "flex", flexWrap: "wrap", gap: 1, marginTop: 1 }}
                          >
                            {newVariantForm.formImages.map((file, index) => (
                              <Box key={index} sx={{ position: "relative" }}>
                                <Box
                                  component="img"
                                  src={variantPreviewUrls[index]}
                                  alt={file.name}
                                  sx={{
                                    width: 80,
                                    height: 80,
                                    objectFit: "cover",
                                    borderRadius: 1,
                                  }}
                                />
                                <IconButton
                                  size="small"
                                  color="error"
                                  onClick={() => handleDeleteVariantUploadedImage(index)}
                                  sx={{
                                    position: "absolute",
                                    top: 0,
                                    right: 0,
                                    backgroundColor: "rgba(255, 255, 255, 0.7)",
                                  }}
                                >
                                  <DeleteIcon fontSize="small" />
                                </IconButton>
                              </Box>
                            ))}
                          </Box>
                        </Box>
                      )}
                    </Grid>
                  </Grid>
                </Box>
              </Grid>
            </Grid>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseForm} variant="outlined">
            Cancel
          </Button>
          <Button
            onClick={handleSaveProduct}
            variant="contained"
            startIcon={<SaveIcon />}
            disabled={
              isCreatingProduct ||
              isUpdatingProduct ||
              isCreatingVariant ||
              isUpdatingVariant ||
              isDeletingVariant
            }
          >
            {(isCreatingProduct ||
              isUpdatingProduct ||
              isCreatingVariant ||
              isUpdatingVariant ||
              isDeletingVariant) ? (
              <CircularProgress size={24} color="inherit" />
            ) : selectedProductId ? (
              "Update"
            ) : (
              "Create"
            )}
          </Button>
        </DialogActions>
      </Dialog>

      <Dialog open={isDeleteDialogOpen} onClose={handleCloseDeleteDialog}>
        <DialogTitle>Confirm Delete</DialogTitle>
        <DialogContent>
          <Typography>
            Are you sure you want to delete this product and all its variants? This action cannot be
            undone.
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDeleteDialog} variant="outlined">
            Cancel
          </Button>
          <Button
            onClick={handleConfirmDelete}
            color="error"
            variant="contained"
            disabled={isDeleting}
          >
            {isDeleting ? <CircularProgress size={24} color="inherit" /> : "Delete"}
          </Button>
        </DialogActions>
      </Dialog>

      <VariantDialog
        open={!!viewingVariants}
        onOpenChange={() => setViewingVariants(null)}
        product={viewingVariants}
      />
    </Box>
  );
}