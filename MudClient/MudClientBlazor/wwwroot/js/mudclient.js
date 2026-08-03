(function () {
	let sendCommandHelper = null;

	window.registerSendCommandHandler = function (dotNetHelper) {
		sendCommandHelper = dotNetHelper;
	};

	window.disposeSendCommandHandler = function () {
		sendCommandHelper = null;
	};

	document.addEventListener('click', function (event) {
		const link = event.target.closest && event.target.closest('[data-mxp-command-id]');
		if (!link) {
			return;
		}

		event.preventDefault();
		const commandId = Number.parseInt(link.dataset.mxpCommandId, 10);
		if (sendCommandHelper && Number.isInteger(commandId)) {
			sendCommandHelper.invokeMethodAsync('SendCommandById', commandId);
		}
	});

	window.mudClientHotkeys = (function () {
		let dotNetHelper = null;
		let keydownHandler = null;
		let enabled = true;
		let boundCodes = new Set();

		function isNumpadCode(code) {
			return code && code.indexOf('Numpad') === 0;
		}

		function isPrintableKey(event) {
			return typeof event.key === 'string' && event.key.length === 1;
		}

		function isEditableTarget(target) {
			return !!(target && target.closest && target.closest('input, textarea, select, [contenteditable]'));
		}

		function shouldIgnoreEvent(event, code) {
			if (!enabled || event.defaultPrevented || event.ctrlKey || event.altKey || event.metaKey || event.shiftKey) {
				return true;
			}

			const target = event.target;
			if (isEditableTarget(target) && isPrintableKey(event) && !isNumpadCode(code)) {
				return true;
			}

			return !!(target && target.closest && target.closest('[data-hotkey-settings="true"]'));
		}

		function handleKeydown(event) {
			const code = event.code || '';
			if (!code || shouldIgnoreEvent(event, code) || !boundCodes.has(code)) {
				return;
			}

			event.preventDefault();
			event.stopPropagation();
			if (event.stopImmediatePropagation) {
				event.stopImmediatePropagation();
			}

			if (dotNetHelper) {
				dotNetHelper.invokeMethodAsync('HandleHotkeyCode', code);
			}
		}

		return {
			register: function (helper) {
				if (keydownHandler) {
					document.removeEventListener('keydown', keydownHandler, true);
				}

				dotNetHelper = helper;
				keydownHandler = handleKeydown;
				document.addEventListener('keydown', keydownHandler, true);
			},
			setBoundCodes: function (codes) {
				boundCodes = new Set(codes || []);
			},
			setEnabled: function (value) {
				enabled = !!value;
			},
			dispose: function () {
				if (keydownHandler) {
					document.removeEventListener('keydown', keydownHandler, true);
				}

				keydownHandler = null;
				dotNetHelper = null;
				boundCodes = new Set();
			}
		};
	})();

	window.mudClientInput = (function () {
		let element = null;
		let handler = null;
		let dotNetHelper = null;

		function requestHistoryNavigation(event, direction) {
			const selectionStart = element.selectionStart;
			const selectionEnd = element.selectionEnd;
			const jumpToBoundary = event.ctrlKey;
			window.requestAnimationFrame(function () {
				if (!element ||
					element.selectionStart !== selectionStart ||
					element.selectionEnd !== selectionEnd ||
					!dotNetHelper) {
					return;
				}

				dotNetHelper.invokeMethodAsync('NavigateCommandHistory', direction, jumpToBoundary);
			});
		}

		return {
			register: function (elementId, helper) {
				this.dispose();
				element = document.getElementById(elementId);
				if (!element) {
					return;
				}
				dotNetHelper = helper;

				handler = function (event) {
					if (event.key === 'Enter' && !event.shiftKey) {
						event.preventDefault();
						return;
					}

					if (!event.shiftKey && !event.altKey && !event.metaKey &&
						(event.key === 'ArrowUp' || event.key === 'ArrowDown')) {
						requestHistoryNavigation(event, event.key === 'ArrowUp' ? 'up' : 'down');
					}
				};
				element.addEventListener('keydown', handler);
			},
			dispose: function () {
				if (element && handler) {
					element.removeEventListener('keydown', handler);
				}

				element = null;
				handler = null;
				dotNetHelper = null;
			}
		};
	})();

	window.mudClientTranscript = (function () {
		let element = null;
		let mutationObserver = null;
		let scrollHandler = null;
		let keydownHandler = null;
		let isPinnedToBottom = true;
		let scrollFrame = null;

		function updatePinnedState() {
			if (!element) {
				return;
			}

			isPinnedToBottom = element.scrollTop + element.clientHeight >= element.scrollHeight - 2;
		}

		function scrollToBottom() {
			if (!element) {
				return;
			}

			element.scrollTop = element.scrollHeight;
			isPinnedToBottom = true;
		}

		function scheduleScrollToBottom() {
			if (scrollFrame !== null || !isPinnedToBottom) {
				return;
			}

			scrollFrame = window.requestAnimationFrame(function () {
				scrollFrame = null;
				if (isPinnedToBottom) {
					scrollToBottom();
				}
			});
		}

		function scrollBy(amount) {
			if (!element) {
				return;
			}

			element.scrollTop += amount;
			updatePinnedState();
		}

		function handleKeydown(event) {
			if (!element || event.altKey || event.metaKey) {
				return;
			}

			switch (event.key) {
				case 'Home':
					event.preventDefault();
					element.scrollTop = 0;
					updatePinnedState();
					break;
				case 'End':
					event.preventDefault();
					scrollToBottom();
					break;
				case 'PageUp':
					event.preventDefault();
					scrollBy(-element.clientHeight);
					break;
				case 'PageDown':
					event.preventDefault();
					scrollBy(element.clientHeight);
					break;
				case 'ArrowUp':
					event.preventDefault();
					scrollBy(-Math.max(24, parseFloat(getComputedStyle(element).lineHeight) || 24));
					break;
				case 'ArrowDown':
					event.preventDefault();
					scrollBy(Math.max(24, parseFloat(getComputedStyle(element).lineHeight) || 24));
					break;
			}
		}

		return {
			register: function (outputElement) {
				this.dispose();
				element = outputElement;
				if (!element) {
					return;
				}

				updatePinnedState();
				scrollToBottom();
				scrollHandler = updatePinnedState;
				keydownHandler = handleKeydown;
				element.addEventListener('scroll', scrollHandler, { passive: true });
				element.addEventListener('keydown', keydownHandler);
				mutationObserver = new MutationObserver(scheduleScrollToBottom);
				mutationObserver.observe(element, { childList: true, subtree: true });
			},
			dispose: function () {
				if (element && scrollHandler) {
					element.removeEventListener('scroll', scrollHandler);
				}

				if (element && keydownHandler) {
					element.removeEventListener('keydown', keydownHandler);
				}

				if (mutationObserver) {
					mutationObserver.disconnect();
				}

				if (scrollFrame !== null) {
					window.cancelAnimationFrame(scrollFrame);
				}

				element = null;
				mutationObserver = null;
				scrollHandler = null;
				keydownHandler = null;
				isPinnedToBottom = true;
				scrollFrame = null;
			}
		};
	})();

	window.getSelectionStart = function (elementId) {
		const element = document.getElementById(elementId);
		return element ? element.selectionStart : 0;
	};

	window.getSelectionEnd = function (elementId) {
		const element = document.getElementById(elementId);
		return element ? element.selectionEnd : 0;
	};

	window.setSelectionRange = function (elementId, startPosition, endPosition) {
		const element = document.getElementById(elementId);
		if (!element) {
			return;
		}

		element.focus();
		const end = typeof endPosition === 'number' ? endPosition : startPosition;
		element.setSelectionRange(startPosition, end);
	};

	window.triggerDownload = function (filename, dataUri) {
		const link = document.createElement('a');
		link.href = dataUri;
		link.download = filename;
		link.click();
	};
})();
